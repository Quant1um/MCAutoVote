using MCAutoVote.Bootstrap;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MCAutoVote.Preferences
{
    public static class Preferences
    {
        public static string FilePath { get; } = Path.Combine(Info.Directory, "preferences.json");

        private static PreferencesData data;
        public static PreferencesData Data
        {
            get
            {
                if (data == null)
                    Load();
                return data;
            }

            set
            {
                data = value;
            }
        }


        public static void Load()
        {
            try
            {
                data = JsonConvert.DeserializeObject<PreferencesData>(File.ReadAllText(FilePath));
                if (data == null) data = new PreferencesData();
            } catch(Exception)
            {
                data = new PreferencesData();
            }
        }

        public static void Save()
        {
            if(data != null) File.WriteAllText(FilePath, JsonConvert.SerializeObject(data));
        }

        public class PreferencesData
        {
            private string nickname;
            private bool hidden;
            private bool enabled;
            private DateTime lastVote;

            public string Nickname
            {
                get
                {
                    return nickname;
                }

                set
                {
                    nickname = value;
                    Save();
                }
            }

            public bool Hidden
            {
                get
                {
                    return hidden;
                }

                set
                {
                    hidden = value;
                    Save();
                }
            }

            public bool Enabled
            {
                get
                {
                    return enabled;
                }

                set
                {
                    enabled = value;
                    Save();
                }
            }


            public DateTime LastVote
            {
                get
                {
                    return lastVote;
                }

                set
                {
                    lastVote = value;
                    Save();
                }
            }

        }
    }
}
