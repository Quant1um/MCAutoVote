using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using MCAutoVote.Voting;
using MCAutoVote.Utilities;
using MCAutoVote.Bootstrap;

namespace MCAutoVote.Interface.CommandControl
{
    [LoadModule]
    public static class Commands
    {
        static Commands()
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

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
        private class AliasAttribute : Attribute
        {
            public string Alias { get; }

            public AliasAttribute(string alias)
            {
                Alias = alias;
            }
        }

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        private class DescriptionAttribute : Attribute
        {
            public string Description { get; }

            public DescriptionAttribute(string desc)
            {
                Description = desc;
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
                Text.Write(string.Join(", ", aliases[name].ToArray()), ConsoleColor.Yellow);
                Text.Write(" - ");
                Text.WriteLine(CommandRegistry.GetDescriptionByName(name) ?? "<No Description Provided>");
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

        [Description("Prints out something...")]
        public static Command Smug = (fullCmd, args) =>
        {
            Text.WriteLine(Properties.Resources.Smug);
        };

        private static bool tooltipShown = false;
        [Alias("h")]
        [Description("Hides the console interface (Double-click to tray icon to show console again).")]
        public static Command Hide = (fullCmd, args) =>
        {
            InterfaceLifecycle.ConsoleHidden = true;

            if (!tooltipShown)
            {
                tooltipShown = true;
                ApplicationContext.Tray.Bubble("Console interface was hidden! To reveal, double click on tray icon.");
                Interface.Text.WriteLine("Console was hidden successfully!", ConsoleColor.Gray);
            }
        };

        [Alias("vote")]
        [Description("Forces to apply voting actions immediately.")]
        public static Command ForceVote = (fullCmd, args) => Vote.Do();

        [Alias("nick")]
        [Description("Sets or gets nickname for voting actions. Required. Usage: nickname [nick]")]
        public static Command Nickname = (fullCmd, args) =>
        {
            if (args.Length >= 1)
            {
                string nick = string.Join(" ", args);
                if (StringUtils.IsNullEmptyOrWhitespace(nick))
                    throw new ArgumentException("Nickname cannot be null, empty or whitespace!");

                Vote.Nickname = nick;
                Text.WriteLine("Nickname has been set to '{0}'", nick);
            }
            else
            {
                string nick = Vote.Nickname;
                if (string.IsNullOrEmpty(nick))
                    Text.WriteLine("Nickname isn't set yet");
                else
                    Text.WriteLine("Current nickname is '{0}'", Properties.Settings.Default.Nickname);
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

                Text.Write("Autostart has been ");
                if (state)
                    Text.Write("enabled", ConsoleColor.Green);
                else
                    Text.Write("disabled", ConsoleColor.Red);
                Text.WriteLine("!");
            }
            else
            {
                Text.Write("Autostart is ");
                if (Bootstrap.Info.Autostart)
                    Text.Write("enabled", ConsoleColor.Green);
                else
                    Text.Write("disabled", ConsoleColor.Red);
                Text.WriteLine("!");
            }
        };

        [Alias("av")]
        [Description("Sets or gets autovote state. Usage: autovote [enable|on|disable|off]")]
        public static Command Autovote = (fullCmd, args) =>
        {
            if (args.Length >= 1)
            {
                bool state = StringUtils.ParseState(args[0], new string[] { "enable", "on" }, new string[] { "disable", "off" });
                Vote.Auto.Enabled = state;
                Text.Write("Autovote has been ");
                if (state)
                    Text.Write("enabled", ConsoleColor.Green);
                else
                    Text.Write("disabled", ConsoleColor.Red);
                Text.WriteLine("!");
            }
            else
            {
                Text.Write("Autovote is ");
                if (Vote.Auto.Enabled)
                    Text.Write("enabled", ConsoleColor.Green);
                else
                    Text.Write("disabled", ConsoleColor.Red);
                Text.WriteLine("!");

                if (Vote.Auto.Enabled)
                {
                    if(!Vote.IsNicknameValid)
                        Text.WriteLine("Nickname is not valid!", ConsoleColor.DarkRed);
                    else
                    {
                        if (Vote.Auto.UntilAction < TimeSpan.FromMinutes(1))
                            Text.WriteLine("Less than minute left!", ConsoleColor.Gray);
                        else
                            Text.WriteLine("{0} left!", ConsoleColor.Gray, StringUtils.GetTimeString(Vote.Auto.UntilAction));
                    }          
                }
            }
        };

        [Description("Dumps settings.")]
        public static Command DumpSettings = (fullCmd, args) =>
        {
            foreach(SettingsProperty prop in Properties.Settings.Default.Properties)
                Text.WriteLine("{0} ({1}) = {2}", prop.Name, prop.PropertyType.Name, Properties.Settings.Default[prop.Name]);
        };                                                                                                                                                                                                                 
    }
}
