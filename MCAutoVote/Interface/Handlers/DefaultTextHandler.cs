using System;

namespace MCAutoVote.Interface
{
    public class DefaultTextHandler : ITextHandler
    {
        public static DefaultTextHandler Instance { get; } = new DefaultTextHandler();

        private DefaultTextHandler() { }

        public void Write(string str, ConsoleColor color, params object[] parameters)
        {
            ConsoleWindow.Foreground = color;
            WriteNoColor(str, parameters);
            ConsoleWindow.Foreground = ConsoleColor.White;
        }

        public static void WriteNoColor(string str, object[] parameters)
        {
            string formatted = str;
            if (parameters != null && parameters.Length > 0)
                formatted = string.Format(str, parameters);
            ConsoleWindow.Write(formatted);
        }
    }
}
