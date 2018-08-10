using System;
using System.Collections.Generic;

namespace MCAutoVote.Interface.CommandControl
{
    public delegate void Command(Arguments args);
    public static class CommandRegistry
    {
        private static IDictionary<string, Command> commands = new Dictionary<string, Command>();
        private static IDictionary<string, string> aliases = new Dictionary<string, string>();
        private static IDictionary<string, string> descriptions = new Dictionary<string, string>();

        public static void AddCommand(string name, Command cmd)
        {
            commands.Add(name, cmd);
            aliases.Add(name, name);
        }

        public static void AddAlias(string originalName, string alias)
        {
            if (!commands.ContainsKey(originalName))
                throw new ArgumentException($"Command with name {originalName} not exists!");
            aliases.Add(alias, originalName);
        }

        public static void AddDescription(string originalName, string description)
        {
            if (!commands.ContainsKey(originalName))
                throw new ArgumentException($"Command with name {originalName} not exists!");
            descriptions.Add(originalName, description);
        }

        public static IEnumerable<string> EnumerateCommands() => commands.Keys;
        public static IEnumerable<string> EnumerateAliases() => aliases.Keys;

        public static string GetNameByAlias(string alias)
        {
            if (aliases.TryGetValue(alias, out string name))
                return name;
            return null;
        }

        public static Command GetCommandByAlias(string alias)
        {
            string name = GetNameByAlias(alias);
            if (name == null) return null;
            return commands[name];
        }

        public static string GetDescriptionByName(string name)
        {
            if (descriptions.TryGetValue(name, out string desc))
                return desc;
            return null;
        }
    }
}
