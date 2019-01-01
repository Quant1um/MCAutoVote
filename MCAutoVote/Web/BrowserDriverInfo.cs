using Microsoft.Win32;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using System;

namespace MCAutoVote.Web
{
    public class BrowserDriverInfo
    {
        public string Path { get; }
        public BrowserDriverType Browser { get; }

        public BrowserDriverInfo(string path, BrowserDriverType browser)
        {
            Path = path;
            Browser = browser;
        }

        public RemoteWebDriver CreateDriver()
        {
            switch(Browser)
            {
                case BrowserDriverType.IE: return new InternetExplorerDriver(Path);
                case BrowserDriverType.Edge: return new EdgeDriver(Path);
                case BrowserDriverType.Chrome: return new ChromeDriver(Path);
                case BrowserDriverType.Safari: return new SafariDriver(Path);
                case BrowserDriverType.Firefox: return new FirefoxDriver(Path);
                default: throw new NotSupportedException("Unsupported browser type!");
            }
        }

        [JsonIgnore]
        public bool IsWebDriverSupported
        {
            get
            {
                switch (Browser)
                {
                    case BrowserDriverType.IE: return true;
                    case BrowserDriverType.Edge: return true;
                    case BrowserDriverType.Chrome: return true;
                    case BrowserDriverType.Safari: return true;
                    case BrowserDriverType.Firefox: return true;
                    default: return false;
                }
            }
        }

        public static BrowserDriverInfo Parse(string query)
        {
            if (query == null)
                throw new ArgumentNullException("Query is null!");

            string[] split = query.Split(new char[] { '@' }, 2);

            if (split.Length != 2)
                throw new ArgumentException("Invalid format. Expected '[ie|edge|chrome|safari|firefox]@path', where browser must be either 'ie', 'edge', 'safari', 'chrome' or 'firefox'");

            BrowserDriverInfo browser = new BrowserDriverInfo(split[1], ParseType(split[0]));
            if (!browser.IsWebDriverSupported)
                throw new ArgumentException("Browser of type '" + split[0] + "' is not supported!");

            return browser;
        }

        private static BrowserDriverType ParseType(string query)
        {
            switch ((query ?? "").ToLower())
            {
                case "ie": return BrowserDriverType.IE;
                case "edge": return BrowserDriverType.Edge;
                case "chrome": return BrowserDriverType.Chrome;
                case "safari": return BrowserDriverType.Safari;
                case "firefox": return BrowserDriverType.Firefox;
                default: return BrowserDriverType.Unknown;
            }
        }

        public override string ToString()
        {
            return Enum.GetName(typeof(BrowserDriverType), Browser).ToLower() + "@" + Path;
        }
    }
}
