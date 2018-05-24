using MCAutoVote.Utilities;
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
            if (RegistryUtils.Autostart[Name] != ExecutablePath)
                RegistryUtils.Autostart[Name] = ExecutablePath;
        }

        public static string Name { get; } = "MCAutoVote";
        public static string Version { get; } = "0.1";
        public static string ExecutablePath { get; } = Assembly.GetEntryAssembly().Location;
        public static string ExecutableName { get; } = Path.GetFileName(ExecutablePath);
        public static string Directory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Name);

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
            get => RegistryUtils.Autostart[Name] == ExecutablePath;
            set => RegistryUtils.Autostart[Name] = value ? ExecutablePath : null;
        }
    }
}
