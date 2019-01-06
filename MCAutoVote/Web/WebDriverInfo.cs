using Microsoft.Win32;
using Newtonsoft.Json;
using System;

namespace MCAutoVote.Web
{
    public struct WebDriverInfo
    {
        private static WebDriverType? GetTypeByProgId(string progId)
        {
            switch(progId)
            {
                case "IE.HTTP": return WebDriverType.IE;
                case "FirefoxURL": return WebDriverType.Firefox;
                case "ChromeHTML": return WebDriverType.Chrome;
                case "SafariHTML": return WebDriverType.Safari;
                case "AppXq0fevzme2pys62n3e0fbqa7peapykr8v": return WebDriverType.Edge;
                default: return null;
            }
        }

        private static string GetProgIdByType(WebDriverType type)
        {
            switch (type)
            {
                case WebDriverType.IE: return "IE.HTTP";
                case WebDriverType.Firefox: return "FirefoxURL";
                case WebDriverType.Chrome: return "ChromeHTML";
                case WebDriverType.Safari: return "SafariHTML";
                case WebDriverType.Edge: return "AppXq0fevzme2pys62n3e0fbqa7peapykr8v";
                default: return null;
            }
        }

        private static string GetPathByProgId(string progId)
        {
            const string exeSuffix = ".exe";
            string path = progId + @"\shell\open\command";
            using (RegistryKey pathKey = Registry.ClassesRoot.OpenSubKey(path))
            {
                if (pathKey == null)
                    return null;

                // Trim parameters.
                try
                {
                    path = pathKey.GetValue(null).ToString().ToLower();
                    if (path.StartsWith("\"") && path.EndsWith("\""))
                        path = path.Substring(1, path.Length - 2);

                    if (!path.EndsWith(exeSuffix))
                        path = path.Substring(0, path.LastIndexOf(exeSuffix, StringComparison.Ordinal) + exeSuffix.Length);

                    return path;
                }
                catch
                {

                    return null;
                }
            }
        }

        public static WebDriverInfo GetDefault()
        {
            try
            {
                const string userChoice = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice";
                using (RegistryKey userChoiceKey = Registry.CurrentUser.OpenSubKey(userChoice))
                {
                    if (userChoiceKey == null)
                        return new WebDriverInfo();

                    object progIdValue = userChoiceKey.GetValue("Progid");
                    if (progIdValue == null)
                        return new WebDriverInfo();

                    string progId = progIdValue.ToString();
                    WebDriverType? type = GetTypeByProgId(progId);
                    if (type == null)
                        return new WebDriverInfo();

                    string path = GetPathByProgId(progId);

                    return new WebDriverInfo(type.Value, path);
                }
            }catch (Exception)
            {
                return new WebDriverInfo();
            }
        }

        public static WebDriverInfo GetByType(WebDriverType type)
        {
            try
            {
                string pid = GetProgIdByType(type);
                string path = GetPathByProgId(pid);

                return new WebDriverInfo(type, path);
            }catch(Exception)
            {
                return new WebDriverInfo(type, null);
            }
        }

        public string Path { get; set; }
        public WebDriverType? Browser { get; set;  }

        public WebDriverInfo(WebDriverType type, string path) : this()
        {
            Browser = type;
            Path = path;
        }


        public WebDriverWrapper CreateWrapper()
        {
            if (!IsWebDriverSupported)
                throw new NotSupportedException();
            return WebDriverWrapper.Create(Browser.Value, Path);
        }

        [JsonIgnore]
        public bool IsWebDriverSupported
        {
            get
            {
                switch (Browser)
                {
                    case WebDriverType.IE: return true;
                    case WebDriverType.Edge: return true;
                    case WebDriverType.Chrome: return true;
                    case WebDriverType.Safari: return true;
                    case WebDriverType.Firefox: return true;
                    default: return false;
                }
            }
        }
    }
}
