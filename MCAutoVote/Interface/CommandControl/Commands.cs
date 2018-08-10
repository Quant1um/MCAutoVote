using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using MCAutoVote.Voting;
using MCAutoVote.Utilities;
using MCAutoVote.Bootstrap;
using System.Threading;
using MCAutoVote.Utilities.Persistency;
using static MCAutoVote.Utilities.Persistency.PersistentContainer;

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
        private static readonly Command help = (args) =>
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
        private static readonly Command clear = (args) => ConsoleWindow.Clear();

        [Alias("q")]
        [Alias("quit")]
        [Alias("shutdown")]
        [Alias("leave")]
        [Description("Kills command interface and returns to your OS.")]
        private static readonly Command exit = (args) => Environment.Exit(0);

        [Description("Prints out smug anime girl's face")]
        private static readonly Command smug = (args) =>
        {
            Text.WriteLine(Properties.Resources.Smug);
        };

        private static bool tooltipShown = false;
        [Alias("h")]
        [Description("Hides the console interface (Double-click to tray icon to show console again).")]
        private static readonly Command hide = (args) =>
        {
            ConsoleWindow.Hidden = true;

            if (!tooltipShown)
            {
                tooltipShown = true;
                ApplicationContext.Tray.Bubble("Console interface was hidden! To reveal, double click on tray icon.");
                Interface.Text.WriteLine("Console was hidden successfully!", ConsoleColor.Gray);
            }
        };

        [Alias("vote")]
        [Description("Forces to apply voting actions immediately.")]
        private static readonly Command forceVote = (args) => Vote.Do();

        [Alias("nick")]
        [Description("Sets or gets nickname for voting actions. Required. Usage: nickname [nick]")]
        private static readonly Command nickname = (args) =>
        {
            if (!string.IsNullOrEmpty(args.ArgQuery))
            {
                string nick = args.ArgQuery;
                if (string.IsNullOrWhiteSpace(nick))
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
                    Text.WriteLine("Current nickname is '{0}'", Vote.Nickname);
            }
        };

        [Alias("as")]
        [Description("Sets or gets autostart state. Usage: autostart [enable|on|disable|off]")]
        private static readonly Command autostart = (args) =>
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
        [Description("Starts automatical voting. Usage: autovote")]
        private static readonly Command autovote = (args) =>
        {
            Text.WriteLine("Autovote started! To stop - press ESCAPE key");

            Anchor anchor = new Anchor();
            Text.WriteLine();

            bool active = true;
            while(active)
            {
                ConsoleKeyInfo? key = ConsoleWindow.ReadKeyIfAvailable();
                if (key.HasValue && key.Value.Key == ConsoleKey.Escape)
                    active = false;

                TimeSpan timeLeft = Vote.UntilAction;

                using (anchor.Use())
                    Text.Write("{0} | {1}", Vote.IsNicknameValid ? ConsoleColor.Cyan : ConsoleColor.Red, Vote.IsNicknameValid ? Vote.Nickname : "Nickname not set", (Vote.IsNicknameValid && timeLeft < TimeSpan.Zero) ? "Voting right now..." : StringUtils.GetTimeString(timeLeft < TimeSpan.Zero ? TimeSpan.Zero : timeLeft) + " left");

                if (Vote.IsNicknameValid && timeLeft < TimeSpan.Zero)
                    Vote.Do();

                Thread.Sleep(500);
            }
        };

        [Description("Dumps preferences & state")]
        private static readonly Command dump = (args) =>
        {
            Text.WriteLine("- State:");
            foreach (Property prop in State.Enumerate())
                Text.WriteLine("    {0} ({1}) = {2} [Default: {3}]", ConsoleColor.Gray, prop.Key, prop.Type.FullName, prop.Value ?? "None", prop.Default ?? "None");
            Text.WriteLine("- Prefs:");
            foreach (Property prop in Preferences.Enumerate())
                Text.WriteLine("    {0} ({1}) = {2} [Default: {3}]", ConsoleColor.Gray, prop.Key, prop.Type.FullName, prop.Value ?? "None", prop.Default ?? "None");
        };                                                                                                                                                                                                                 
    }
}
