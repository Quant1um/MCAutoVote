using System;

namespace MCAutoVote.CLI
{
    public static class CLIOutput
    {
        public static void Write(string str, ConsoleColor color, params object[] parameters)
        {
            Console.ForegroundColor = color;
            Console.Write(parameters.Length == 0 ? str : string.Format(str, parameters));
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void Write(string str, params object[] parameters) => Write(str, ConsoleColor.White, parameters);
        public static void Write(string str) => Write(str, ConsoleColor.White);

        public static void WriteLine(string str, ConsoleColor color, params object[] parameters) => Write(str + Environment.NewLine, color, parameters);
        public static void WriteLine(string str, params object[] parameters) => WriteLine(str, ConsoleColor.White, parameters);
        public static void WriteLine(string str) => WriteLine(str, ConsoleColor.White);
        public static void WriteLine() => Console.WriteLine();
    }
}
