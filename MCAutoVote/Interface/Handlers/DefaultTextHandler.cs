using System;

namespace MCAutoVote.Interface
{
    public class DefaultTextHandler : ITextHandler
    {
        public static DefaultTextHandler Instance { get; } = new DefaultTextHandler();

        private DefaultTextHandler() { }

        public void Write(string str, ConsoleColor color, params object[] parameters)
        {
            Console.ForegroundColor = color;
            WriteNoColor(str, parameters);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void WriteNoColor(string str, object[] parameters)
        {
            string formatted = str;
            if (parameters != null && parameters.Length > 0)
                formatted = string.Format(str, parameters);
            Console.Write(formatted);
        }
    }
}
