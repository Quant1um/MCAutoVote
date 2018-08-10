using System;
using MCAutoVote.Bootstrap;
using MCAutoVote.Utilities.Persistency;
using static MCAutoVote.Utilities.NativeMethods;

namespace MCAutoVote.Interface
{
    [LoadModule]
    public static class ConsoleWindow
    {
        static ConsoleWindow()
        {
            Allocate();
            UpdateConsoleState();
        }

        public static IntPtr Handle => GetConsoleWindow();

        public static bool Hidden
        {
            get => Preferences.Hidden;
            set
            {
                Preferences.Hidden = value;
                UpdateConsoleState();
            }
        }

        private static void Allocate()
        {
            if (!AttachConsole(-1)) AllocConsole();
        }
        private static void UpdateConsoleState() =>
             ShowWindow(Handle, Hidden ? SW_HIDE : SW_SHOW);

        public static void Write(string str) => Console.Write(str);
        public static string ReadLine() => Console.ReadLine();
        public static void Clear() => Console.Clear();

        public static string Title
        {
            get => Console.Title;
            set => Console.Title = value;
        }

        public static int CursorLeft
        {
            get => Console.CursorLeft;
            set => Console.CursorLeft = value;
        }

        public static int CursorTop
        {
            get => Console.CursorTop;
            set => Console.CursorTop = value;
        }

        public static ConsoleKeyInfo? ReadKeyIfAvailable()
        {
            if (!Console.KeyAvailable) return null;
            return Console.ReadKey();
        }

        public static ConsoleColor Foreground
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }

        public static ConsoleColor Background
        {
            get => Console.BackgroundColor;
            set => Console.BackgroundColor = value;
        }

        public static event ConsoleCancelEventHandler CancelKeyPress {
            add => Console.CancelKeyPress += value;
            remove => Console.CancelKeyPress -= value;
        }
    }
}
