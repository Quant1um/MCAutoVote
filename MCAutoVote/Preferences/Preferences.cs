using MCAutoVote.Bootstrap;
using MCAutoVote.Preferences.Editor;
using MCAutoVote.Preferences.Reflection;
using MCAutoVote.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
                {
                    Load();
                }
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
                if (data == null)
                {
                    data = new PreferencesData();
                }
                data.Fix();
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
            [JsonIgnore]
            private Editor.Editor _editor;

            [JsonIgnore]
            public Editor.Editor Editor
            {
                get
                {
                    if(_editor == null)
                        _editor = new Editor.Editor(this);
                    return _editor;
                }
            }

            private string nickname = null;
            private bool hidden = false;
            private bool autostart = true;
            private bool enabled = true;
            private DateTime lastVote = DateTime.MinValue;
            private BrowserDriverInfo browser = null;
            
            [EditablePreference(CanUnset = true)]
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

            [EditablePreference]
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

            [EditablePreference]
            public bool Autostart
            {
                get
                {
                    return autostart;
                }

                set
                {
                    autostart = value;
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

            [EditablePreference(CanUnset = true)]
            public BrowserDriverInfo Browser
            {
                get
                {
                    return browser;
                }

                set
                {
                    browser = value;
                    Save();
                }
            }

            public void Fix()
            {
                if(Browser != null && !Browser.IsWebDriverSupported)
                {
                    Browser = null;
                }
            }
        }
    }
}
