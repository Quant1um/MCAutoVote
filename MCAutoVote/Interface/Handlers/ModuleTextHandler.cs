using System;

namespace MCAutoVote.Interface
{
    public class ModuleTextHandler : ITextHandler
    {
        public const string Indent = "    ";
        public static ModuleTextHandler Instance { get; } = new ModuleTextHandler();

        private ModuleTextHandler() { }

        public void Write(string str, ConsoleColor color, params object[] parameters)
        {
            Console.ForegroundColor = Darker(color);
            Console.Write(Indent);
            DefaultTextHandler.Instance.WriteNoColor(str, parameters);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static ConsoleColor Darker(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                case ConsoleColor.DarkBlue:
                case ConsoleColor.DarkGreen:
                case ConsoleColor.DarkCyan:
                case ConsoleColor.DarkRed:
                case ConsoleColor.DarkMagenta:
                case ConsoleColor.DarkYellow:
                case ConsoleColor.DarkGray:     return color;
                case ConsoleColor.Gray:         return ConsoleColor.DarkGray;
                case ConsoleColor.Blue:         return ConsoleColor.DarkBlue;
                case ConsoleColor.Green:        return ConsoleColor.DarkGreen;
                case ConsoleColor.Cyan:         return ConsoleColor.DarkCyan;
                case ConsoleColor.Red:          return ConsoleColor.DarkRed;
                case ConsoleColor.Magenta:      return ConsoleColor.DarkMagenta;
                case ConsoleColor.Yellow:       return ConsoleColor.DarkYellow;
                case ConsoleColor.White:        return ConsoleColor.Gray;
                default: throw new ArgumentException($"Invalid color supplied: {color}");
            }
        }
    }
}
