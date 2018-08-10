using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MCAutoVote.Interface.CommandControl
{
    public class Arguments
    {
        public static char[] CommandSeparators { get; } = { ' ', '\t' };
        public static Regex FlagExpression { get; } = new Regex("^-(.*)", RegexOptions.IgnoreCase);

        private readonly List<string> arguments = new List<string>();
        private readonly ISet<string> flags = new HashSet<string>();

        public string Command { get; }
        public string ArgQuery { get; }
        public string FullQuery { get; }

        public Arguments(string query)
        {
            if (query == null) throw new ArgumentNullException("Query is null!");

            string[] tokens = query.Split(CommandSeparators, 2, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0) throw new ArgumentException("Query is empty!");

            string command = tokens[0].ToLower();
            string args = tokens.Length > 1 ? tokens[1] : "";
            string[] argsTokens = args.Split(CommandSeparators, StringSplitOptions.RemoveEmptyEntries);

            Command = command;
            ArgQuery = args;
            FullQuery = query;

            foreach(string arg in argsTokens)
            {
                Match match = FlagExpression.Match(arg);
                if (match.Success)
                    flags.Add(match.Captures[0].Value.ToLower());
                else
                    arguments.Add(arg);
            }
        }

        public int Length => arguments.Count;

        public bool HasFlag(string flag)
            => flags.Contains((flag ?? throw new ArgumentNullException("Given flag is null")).ToLower());

        public string this[int idx] => Get(idx);
        public string Get(int idx)
        {
            if (idx < 0) throw new ArgumentOutOfRangeException("Argument index cannot be lower than zero!");
            if (idx >= arguments.Count) return null;
            return arguments[idx];
        }
    }
}
