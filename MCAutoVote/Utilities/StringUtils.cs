using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCAutoVote.Utilities
{
    public static class StringUtils
    {
        //damerau-levenshtein string distance
        //https://gist.github.com/wickedshimmy/449595/cb33c2d0369551d1aa5b6ff5e6a802e21ba4ad5c
        public static int EditDistance(string original, string modified)
        {
            int len_orig = original.Length;
            int len_diff = modified.Length;

            var matrix = new int[len_orig + 1, len_diff + 1];
            for (int i = 0; i <= len_orig; i++)
                matrix[i, 0] = i;
            for (int j = 0; j <= len_diff; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= len_orig; i++)
            {
                for (int j = 1; j <= len_diff; j++)
                {
                    int cost = modified[j - 1] == original[i - 1] ? 0 : 1;
                    var vals = new int[] {
                        matrix[i - 1, j] + 1,
                        matrix[i, j - 1] + 1,
                        matrix[i - 1, j - 1] + cost
                    };

                    matrix[i, j] = vals.Min();
                    if (i > 1 && j > 1 && original[i - 1] == modified[j - 2] && original[i - 2] == modified[j - 1])
                        matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + cost);
                }
            }
            return matrix[len_orig, len_diff];
        }

        public static string GetTimeString(TimeSpan span)
        {
            List<string> tokens = new List<string>();

            if (span.Days > 0) tokens.Add(span.Days + " day" + (span.Days > 1 ? "s" : ""));
            if (span.Hours > 0) tokens.Add(span.Hours + " hour" + (span.Hours > 1 ? "s" : ""));
            if (span.Minutes > 0) tokens.Add(span.Minutes + " minute" + (span.Minutes > 1 ? "s" : ""));
            if (span.Seconds > 0) tokens.Add(span.Seconds + " second" + (span.Seconds > 1 ? "s" : ""));

            if (tokens.Count == 0)
                return null;

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < tokens.Count; i++)
            {
                builder.Append(tokens[i]);

                if (i < tokens.Count - 1)
                {
                    if (i == tokens.Count - 2)
                        builder.Append(" and ");
                    else
                        builder.Append(", ");
                }
            }

            return builder.ToString();
        }

        public static bool ParseState(string input, string[] truly, string[] falsy)
        {
            if (TryParseState(input, truly, falsy, out bool state))
                return state;
            else
                throw new ArgumentException("Invalid token!");
        }

        public static bool TryParseState(string input, string[] truly, string[] falsy, out bool result)
        {
            result = false;
            if (input == null) return false;
            input = input.ToLower();

            if (truly.Any(f => f.ToLower() == input))
                result = true;
            else if (falsy.Any(f => f.ToLower() == input))
                result = false;
            else
                return false;
            return true;
        }

        public static bool IsNullEmptyOrWhitespace(string str)
        {
            return str == null || str.Trim().Length == 0;
        }
    }
}
