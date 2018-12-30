using MCAutoVote.Bootstrap;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MCAutoVote.CLI
{
    [LoadModule]
    public static class CLIWindow
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        static CLIWindow()
        {
            UpdateWindowState();
        }

        public static IntPtr Window => GetConsoleWindow();

        public static bool Hidden
        {
            get => Preferences.Preferences.Data.Hidden;
            set
            {
                Preferences.Preferences.Data.Hidden = value;
                UpdateWindowState();
            }
        }

        private static void UpdateWindowState()
        {
            ShowWindow(Window, Hidden ? SW_HIDE : SW_SHOW);
        }
    }
}
