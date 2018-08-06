using MCAutoVote.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MCAutoVote.Bootstrap
{
    public static class Info
    {
        public static string Name { get; } = "MCAutoVote";
        public static string Version { get; } = "0.1";
        public static string ExecutablePath { get; } = Assembly.GetEntryAssembly().Location;
        public static string ExecutableName { get; } = Path.GetFileName(ExecutablePath);
        public static string Directory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Name);

        public static string FileStacktrace { get; }    = Path.Combine(Directory, "stacktrace.log");
        public static string FilePreferences { get; }   = Path.Combine(Directory, "prefs.xml");
        public static string FileState { get; }         = Path.Combine(Directory, "state.xml");

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
                List<String> tokens = new List<string> { Name };

                if (Version != null) tokens.Add("v" + Version);
                if (DevelopmentEnvironment) tokens.Add("devenv");

                return string.Join(" ", tokens);
            }
        }

        public static bool Autostart
        {
            get => RegistryUtils.Autostart[Name] != null;
            set => RegistryUtils.Autostart[Name] = value ? ExecutablePath : null;
        }
    }
}
