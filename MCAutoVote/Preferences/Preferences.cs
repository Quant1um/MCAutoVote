using MCAutoVote.Bootstrap;
using MCAutoVote.Preferences.Editor;
using MCAutoVote.Web;
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

                try
                {
                    Save();
                }
                catch (Exception) { }
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
                    if (_editor == null)
                        _editor = new Editor.Editor(this);
                    return _editor;
                }
            }

            private string nickname = null;
            private bool hidden = false;
            private bool autostart = true;
            private bool enabled = true;
            private bool lastTimeFailed = false;
            private DateTime lastVote = DateTime.MinValue;
            private WebDriverInfo driverInfo = WebDriverInfo.GetDefault();

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

            public bool LastTimeFailed
            {
                get
                {
                    return lastTimeFailed;
                }

                set
                {
                    lastTimeFailed = value;
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

            public WebDriverInfo DriverInfo
            {
                get
                {
                    return driverInfo;
                }

                set
                {
                    driverInfo = value;
                    Save();
                }
            }

            [JsonIgnore]
            [EditablePreference(CanUnset = true, Name = "browser")]
            private WebDriverType? BrowserType
            {
                get
                {
                    return driverInfo.Browser;
                }

                set
                {
                    if (value == null)
                        driverInfo = new WebDriverInfo();
                    else
                        driverInfo = WebDriverInfo.GetByType(value.Value);
                    Save();
                }
            }

            [JsonIgnore]
            [EditablePreference(CanUnset = true, Name = "browserPath")]
            private string BrowserPath
            {
                get
                {
                    return driverInfo.Path;
                }

                set
                {
                    driverInfo.Path = value;
                    Save();
                }
            }

            public void Fix()
            {
                if(!driverInfo.IsWebDriverSupported)
                {
                    driverInfo = WebDriverInfo.GetDefault();
                }
            }
        }
    }
}
