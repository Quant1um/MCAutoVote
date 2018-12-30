using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using MCAutoVote.Voting;
using MCAutoVote.Utilities;
using Newtonsoft.Json;

namespace MCAutoVote.CLI.Command
{
    public static class Commands
    {
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
        public class AliasAttribute : Attribute
        {
            public string Alias { get; }

            public AliasAttribute(string alias)
            {
                Alias = alias;
            }
        }

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class DescriptionAttribute : Attribute
        {
            public string Description { get; }

            public DescriptionAttribute(string desc)
            {
                Description = desc;
            }
        }

        public static void RegisterAll()
        {
            foreach (FieldInfo f in typeof(Commands).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Where(f => f.FieldType == typeof(Command)))
            {
                string name = f.Name.ToLower();
                CommandRegistry.AddCommand(name, (Command)f.GetValue(null));
                foreach (AliasAttribute attr in f.GetCustomAttributes(typeof(AliasAttribute), false))
                    CommandRegistry.AddAlias(name, attr.Alias.ToLower());
                DescriptionAttribute descriptionAttribute = f.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().FirstOrDefault();
                if (descriptionAttribute != null)
                    CommandRegistry.AddDescription(name, descriptionAttribute.Description);
            }
        }

        [Alias("commands")]
        [Alias("cmds")]
        [Description("Prints out all supported commands.")]
        public static Command Help = (full, args) =>
        {
            IDictionary<string, List<string>> aliases = new Dictionary<string, List<string>>();
            foreach (string alias in CommandRegistry.EnumerateAliases())
            {
                string name = CommandRegistry.GetNameByAlias(alias);
                if (!aliases.TryGetValue(name, out List<string> list))
                    aliases.Add(name, list = new List<string>());
                list.Add(alias);
            }

            foreach (string name in aliases.Keys)
            {
                CLIOutput.Write(string.Join(", ", aliases[name].ToArray()), ConsoleColor.Yellow);
                CLIOutput.Write(" - ");
                CLIOutput.WriteLine(CommandRegistry.GetDescriptionByName(name) ?? "<No Description Provided>");
            }
        };

        [Alias("c")]
        [Alias("cl")]
        [Alias("clean")]
        [Alias("vanish")]
        [Description("Clears all contents of console interface.")]
        public static Command Clear = (fullCmd, args) => Console.Clear();

        [Alias("q")]
        [Alias("quit")]
        [Alias("shutdown")]
        [Alias("leave")]
        [Description("Kills command interface and returns to your OS.")]
        public static Command Exit = (fullCmd, args) => Environment.Exit(0);

        [Description("smug")]
        public static Command Smug = (fullCmd, args) =>
        {
            CLIOutput.WriteLine(Properties.Resources.Smug);
        };

        private static bool tooltipShown = false;
        [Alias("h")]
        [Alias("hh")] //f
        [Description("Hides the console interface (Double-click to tray icon to show console again).")]
        public static Command Hide = (fullCmd, args) =>
        {
            CLIWindow.Hidden = true;

            if (!tooltipShown)
            {
                tooltipShown = true;
                CLITray.Bubble("Console interface was hidden! To reveal, double click on tray icon.");
            }
        };

        [Alias("vote")]
        [Description("Forces to apply voting actions immediately.")]
        public static Command ForceVote = (fullCmd, args) => VoteLoop.Vote();

        [Alias("nick")]
        [Description("Sets or gets nickname for voting actions. Required. Usage: nickname [nick]")]
        public static Command Nickname = (fullCmd, args) =>
        {
            if (args.Length >= 1)
            {
                string nick = string.Join(" ", args);
                if (StringUtils.IsNullEmptyOrWhitespace(nick))
                    throw new ArgumentException("Nickname cannot be null, empty or whitespace!");

                VoteLoop.Nickname = nick;
                CLIOutput.WriteLine("Nickname has been set to '{0}'", nick);
            }
            else
            {
                string nick = VoteLoop.Nickname;
                if (string.IsNullOrEmpty(nick))
                    CLIOutput.WriteLine("Nickname isn't set yet");
                else
                    CLIOutput.WriteLine("Current nickname is '{0}'", VoteLoop.Nickname);
            }
        };

        [Alias("as")]
        [Description("Sets or gets autostart state. Usage: autostart [enable|on|disable|off]")]
        public static Command Autostart = (fullCmd, args) =>
        {
            if (args.Length >= 1)
            {
                bool state = StringUtils.ParseState(args[0], new string[] { "enable", "on" }, new string[] { "disable", "off" });
                Bootstrap.Info.Autostart = state;

                CLIOutput.Write("Autostart has been ");
                if (state)
                    CLIOutput.Write("enabled", ConsoleColor.Green);
                else
                    CLIOutput.Write("disabled", ConsoleColor.Red);
                CLIOutput.WriteLine("!");
            }
            else
            {
                CLIOutput.Write("Autostart is ");
                if (Bootstrap.Info.Autostart)
                    CLIOutput.Write("enabled", ConsoleColor.Green);
                else
                    CLIOutput.Write("disabled", ConsoleColor.Red);
                CLIOutput.WriteLine("!");
            }
        };

        [Alias("av")]
        [Description("Sets or gets autovote state. Usage: autovote [enable|on|disable|off]")]
        public static Command Autovote = (fullCmd, args) =>
        {
            if (args.Length >= 1)
            {
                bool state = StringUtils.ParseState(args[0], new string[] { "enable", "on" }, new string[] { "disable", "off" });
                VoteLoop.Enabled = state;
                CLIOutput.Write("Autovote has been ");
                if (state)
                    CLIOutput.Write("enabled", ConsoleColor.Green);
                else
                    CLIOutput.Write("disabled", ConsoleColor.Red);
                CLIOutput.WriteLine("!");
            }
            else
            {
                CLIOutput.Write("Autovote is ");
                if (VoteLoop.Enabled)
                    CLIOutput.Write("enabled", ConsoleColor.Green);
                else
                    CLIOutput.Write("disabled", ConsoleColor.Red);
                CLIOutput.WriteLine("!");

                if (VoteLoop.Enabled)
                {
                    CLIOutput.WriteLine("State: {0}", ConsoleColor.Gray, VoteLoop.StateString);
                }
            }
        };

        [Description("Dumps settings.")]
        public static Command DumpSettings = (fullCmd, args) =>
        {
            CLIOutput.WriteLine(JsonConvert.SerializeObject(Preferences.Preferences.Data, Formatting.Indented));
        };                                                                                                                                                                                                                 
    }
}
