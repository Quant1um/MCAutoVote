using System;
using System.Collections.Generic;

namespace MCAutoVote.CLI.Command
{
    public delegate void CommandDelegate(string fullCmd, string[] args);
    public static class CommandRegistry
    {
        public static AutocompletionHandler Autocompletion { get; } = new AutocompletionHandler();

        private static IList<Command> commands = new List<Command>();
        private static IDictionary<string, Command> aliasTable = new Dictionary<string, Command>();
        
        public static void AddCommand(Command cmd)
        {
            if (cmd.Aliases.Count == 0)
                throw new ArgumentException("Command must have at least 1 alias!");

            commands.Add(cmd);
            foreach (string alias in cmd.Aliases)
            {
                aliasTable.Add(alias.ToLower(), cmd);
            }
        }
        
        public static IEnumerable<Command> EnumerateCommands()
        {
            return commands;
        }

        public static IEnumerable<string> EnumerateAliases()
        {
            return aliasTable.Keys;
        }

        public static Command GetCommandByAlias(string alias)
        {
            if(aliasTable.TryGetValue(alias.ToLower(), out Command command))
                return command;
            return null;
        }

        public class AutocompletionHandler : IAutoCompleteHandler
        {
            public char[] Separators { get; set; } = new char[] { ' ' };

            public string[] GetSuggestions(string text, int index)
            {
                List<string> suggestions = new List<string>();
                foreach(string alias in EnumerateAliases())
                {
                    if (alias.ToLowerInvariant().StartsWith(text))
                    {
                        suggestions.Add(alias);
                    }
                }

                return suggestions.ToArray();
            }
        }
    }
}
