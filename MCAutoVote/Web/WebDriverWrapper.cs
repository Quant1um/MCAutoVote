using MCAutoVote.Bootstrap;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using System;
using System.IO;

namespace MCAutoVote.Web
{
    [LoadModule]
    public abstract class WebDriverWrapper : IDisposable
    {
        public static string DriversDirectory { get; } = Path.Combine(Info.Directory, "Drivers");
        public static string ProfilesDirectory { get; } = Path.Combine(Info.Directory, "Profiles");

        static WebDriverWrapper()
        {
            Directory.CreateDirectory(DriversDirectory);
            Directory.CreateDirectory(ProfilesDirectory);
        }

        public static WebDriverWrapper Create(WebDriverType type, string path)
        {
            switch(type)
            {
                case WebDriverType.Chrome: return new ChromeWrapper(path);
                case WebDriverType.Firefox: return new FirefoxWrapper(path);
                case WebDriverType.Edge: return new EdgeWrapper();
                case WebDriverType.IE: return new IEWrapper();
                case WebDriverType.Safari: return new SafariWrapper();
                default: throw new NotSupportedException();
            }
        }

        public RemoteWebDriver Driver { get; }

        public WebDriverWrapper(RemoteWebDriver driver)
        {
            Driver = driver;
            Driver.Manage().Window.Minimize();
        }

        public void Dispose()
        {
            Driver.Dispose();
        }

        private class ChromeWrapper : WebDriverWrapper
        {
            public ChromeWrapper(string path) : base(CreateDriver(path)) { }

            private static RemoteWebDriver CreateDriver(string path)
            {
                ChromeOptions options = new ChromeOptions();

                if (path != null)
                    options.BinaryLocation = path;

                options.AddArgument("user-data-dir=" + Path.Combine(ProfilesDirectory, "Chrome"));
                options.AddArgument("profile-directory=Chrome Autovote Profile");

                return new ChromeDriver(DriversDirectory, options);
            }
        }

        private class FirefoxWrapper : WebDriverWrapper
        {
            public FirefoxWrapper(string path) : base(CreateDriver(path)) { }

            private static RemoteWebDriver CreateDriver(string path)
            {
                FirefoxOptions options = new FirefoxOptions();

                if (path != null)
                    options.BrowserExecutableLocation = path;

                FirefoxProfile profile = new FirefoxProfile(Path.Combine(ProfilesDirectory, "Firefox"));
                options.Profile = profile;

                return new FirefoxDriver(DriversDirectory, options);
            }
        }

        private class IEWrapper : WebDriverWrapper
        {
            public IEWrapper() : base(CreateDriver()) { }

            private static RemoteWebDriver CreateDriver()
            {
                return new InternetExplorerDriver(DriversDirectory);
            }
        }

        private class EdgeWrapper : WebDriverWrapper
        {
            public EdgeWrapper() : base(CreateDriver()) { }

            private static RemoteWebDriver CreateDriver()
            {
                return new EdgeDriver(DriversDirectory);
            }
        }

        private class SafariWrapper : WebDriverWrapper
        {
            public SafariWrapper() : base(CreateDriver()) { }

            private static RemoteWebDriver CreateDriver()
            {
                return new SafariDriver(DriversDirectory);
            }
        }
    }
}
