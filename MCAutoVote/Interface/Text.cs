using System;

namespace MCAutoVote.Interface
{
    public static class Text
    {
        public static ITextHandler Handler { get; set; } = DefaultTextHandler.Instance;

        public static void Write(string str, ConsoleColor color, params object[] parameters) => Handler.Write(str, color, parameters);
        public static void Write(string str, params object[] parameters) => Write(str, ConsoleColor.White, parameters);
        public static void Write(string str) => Write(str, ConsoleColor.White);

        public static void WriteLine(string str, ConsoleColor color, params object[] parameters) => Write(str + Environment.NewLine, color, parameters);
        public static void WriteLine(string str, params object[] parameters) => WriteLine(str, ConsoleColor.White, parameters);
        public static void WriteLine(string str) => WriteLine(str, ConsoleColor.White);
        public static void WriteLine() => Write(Environment.NewLine);
    }
}
