using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using MCAutoVote.Voting;
using MCAutoVote.Utilities;
using Newtonsoft.Json;
using MCAutoVote.Bootstrap;

namespace MCAutoVote.CLI.Command
{
    [LoadModule]
    public static class Commands
    {
        static Commands()
        {
            RegisterAll();
        }

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
        public class HiddenAttribute : Attribute { }

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
            foreach (FieldInfo f in typeof(Commands).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Where(f => f.FieldType == typeof(CommandDelegate)))
                CommandRegistry.AddCommand(Command.ByStaticDelegatedField(f));
        }

        [Alias("help")]
        [Alias("commands")]
        [Alias("cmds")]
        [Description("Prints out all supported commands.")]
        public static CommandDelegate Help = (full, args) =>
        {
            foreach (Command command in CommandRegistry.EnumerateCommands())
            {
                if (command.IsHidden)
                    continue;

                CLIOutput.Write(string.Join(", ", command.Aliases.ToArray()), ConsoleColor.Yellow);
                CLIOutput.Write(" - ");
                CLIOutput.WriteLine(command.Description ?? "<No Description Provided>");
            }
        };
        
        [Alias("c")]
        [Alias("cl")]
        [Alias("clean")]
        [Alias("vanish")]
        [Alias("clear")]
        [Description("Clears all contents of console interface.")]
        public static CommandDelegate Clear = (fullCmd, args) => Console.Clear();

        [Alias("q")]
        [Alias("quit")]
        [Alias("shutdown")]
        [Alias("leave")]
        [Alias("exit")]
        [Description("Kills command interface and returns to your OS.")]
        public static CommandDelegate Exit = (fullCmd, args) => Environment.Exit(0);

        [Hidden]
        [Alias("smug")]
        public static CommandDelegate Smug = (fullCmd, args) =>
        {
            CLIOutput.WriteLine(Properties.Resources.Smug);
        };

        private static bool tooltipShown = false;
        [Alias("h")]
        [Alias("hh")]
        [Alias("hide")] //f
        [Description("Hides the console interface (Double-click to tray icon to show console again).")]
        public static CommandDelegate Hide = (fullCmd, args) =>
        {
            CLIWindow.Hidden = true;

            if (!tooltipShown)
            {
                tooltipShown = true;
                CLITray.Bubble("Console interface was hidden! To reveal, double click on tray icon.");
            }
        };

        [Alias("vote")]
        [Alias("forcevote")]
        [Description("Forces to apply voting actions immediately.")]
        public static CommandDelegate ForceVote = (fullCmd, args) => VoteLoop.Vote();

        [Alias("nick")]
        [Alias("nickname")]
        [Description("Sets or gets nickname for voting actions. Required. Usage: nickname [nick]")]
        public static CommandDelegate Nickname = (fullCmd, args) =>
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
        [Alias("autostart")]
        [Description("Sets or gets autostart state. Usage: autostart [enable|on|disable|off]")]
        public static CommandDelegate Autostart = (fullCmd, args) =>
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
        [Alias("autovote")]
        [Description("Sets or gets autovote state. Usage: autovote [enable|on|disable|off]")]
        public static CommandDelegate Autovote = (fullCmd, args) =>
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

        [Alias("p")]
        [Alias("prefs")]
        [Alias("preferences")]
        [Description("Used for managing preferences.")]
        public static CommandDelegate PreferencesCommand = (fullCmd, args) =>
        {
            if(args.Length >= 1)
            {
                switch(args[0].ToLower())
                {
                    case "set":
                        {
                            if (args.Length == 3)
                            {
                                string name = args[1];
                                string value = args[2];
                                string old = FormatPreference(Preferences.Preferences.Data.Editor.Set(name, value));
                                CLIOutput.WriteLine("Preference '{0}' has been set to '{2}'. Old value was '{1}'", ConsoleColor.Gray, name, old, value);
                                return;
                            }
                            break;
                        }

                    case "unset":
                        {
                            if (args.Length == 2)
                            {
                                string name = args[1];
                                string old = FormatPreference(Preferences.Preferences.Data.Editor.Unset(name));
                                CLIOutput.WriteLine("Preference '{0}' has been unset. Old value was '{1}'", ConsoleColor.Gray, name, old);
                                return;
                            }
                            break;
                        }

                    case "view":
                        {
                            if (args.Length == 1)
                            {
                                foreach(KeyValuePair<string, string> pref in Preferences.Preferences.Data.Editor.View())
                                    CLIOutput.WriteLine("{0} = {1}", pref.Value == null ? ConsoleColor.DarkGray : ConsoleColor.Gray, pref.Key, FormatPreference(pref.Value));
                                return;
                            }
                            break;
                        }
                }
            }

            throw new ArgumentException("Usage: prefs <set|unset|view> [name] [value]");
        };

        private static string FormatPreference(string pref) => pref ?? "<NOT SET>";

        [Alias("dumpsettings")]
        [Description("Dumps settings.")]
        public static CommandDelegate DumpSettings = (fullCmd, args) =>
        {
            CLIOutput.WriteLine(JsonConvert.SerializeObject(Preferences.Preferences.Data, Formatting.Indented));
        };                                                                                                                                                                                                                 
    }
}
