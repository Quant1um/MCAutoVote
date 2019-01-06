using MCAutoVote.CLI;
using MCAutoVote.Utilities;
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace MCAutoVote.Bootstrap
{
    [LoadModule]
    public static class Info
    {
        static Info()
        {
            System.IO.Directory.CreateDirectory(Directory);

            try
            {
                if (Autostart && GetAutostartPath(Name) != ExecutablePath)
                    SetAutostartPath(Name, ExecutablePath);
            }catch(Exception e)
            {

                CLIOutput.WriteLine("Failed to set autostart path: probably program doesn't have permissions to do that!", ConsoleColor.DarkYellow);
#if DEBUG
                CLIOutput.WriteLine(e.StackTrace);
#endif
            }
        }

        private static string GetAutostartPath(string name)
        {
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                return (string)rk.GetValue(name);
        }

        private static void SetAutostartPath(string name, string path)
        {
            try
            {
                using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (path != null)
                        rk.SetValue(name, path, RegistryValueKind.String);
                    else
                        rk.DeleteValue(name);
                }
            }catch(Exception)
            {
                CLIOutput.WriteLine("Failed to set autostart path: probably program doesn't have permissions to do that!", ConsoleColor.DarkYellow);
            }
        }

        public static string Name { get; } = "MCAutoVote";
        public static string Version { get; } = "0.1";
        public static string ExecutablePath { get; } = Assembly.GetEntryAssembly().Location;
        public static string ExecutableName { get; } = Path.GetFileName(ExecutablePath);
        public static string Directory { get; } = Path.Combine(Path.GetDirectoryName(ExecutablePath), "Data");

        public static bool DevelopmentEnvironment
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public static string FullName
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(Name);

                if (Version != null)
                {
                    builder.Append(" v").Append(Version);
                }

                if (DevelopmentEnvironment)
                {
                    if (Version != null)
                        builder.Append("-");
                    else
                        builder.Append(" ");
                    builder.Append("devenv");
                }

                return builder.ToString();
            }
        }

        public static bool Autostart
        {
            get => Preferences.Preferences.Data.Autostart;
            set
            {
                if(value)
                    SetAutostartPath(Name, ExecutablePath);
                else
                    SetAutostartPath(Name, null);
                Preferences.Preferences.Data.Autostart = value;
            }
        }
    }
}
