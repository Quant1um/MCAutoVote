using Microsoft.Win32;

namespace MCAutoVote.Utilities
{
    public static class RegistryUtils
    {
        public static BrowserEmulationRegistryKey BrowserEmulation { get; } = new BrowserEmulationRegistryKey();

        public class BrowserEmulationRegistryKey
        {
            public static uint Value { get; } = 11001;

            //https://weblog.west-wind.com/posts/2011/May/21/Web-Browser-Control-Specifying-the-IE-Version
            public bool this[string exename]
            {
                get
                {
                    using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", RegistryKeyPermissionCheck.ReadSubTree))
                        return (uint)rk.GetValue(exename) == Value;
                }

                set
                {
                    using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", RegistryKeyPermissionCheck.ReadWriteSubTree))
                    {
                        if (value)
                            rk.SetValue(exename, Value, RegistryValueKind.DWord);
                        else
                            rk.DeleteValue(exename);
                    }
                }
            }
        }

        public static AutostartRegistryKey Autostart { get; } = new AutostartRegistryKey();

        public class AutostartRegistryKey
        {
            public string this[string appname]
            {
                get
                {
                    using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", RegistryKeyPermissionCheck.ReadSubTree))
                        return (string)rk.GetValue(appname);
                }

                set
                {
                    using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", RegistryKeyPermissionCheck.ReadWriteSubTree))
                    {
                        if (value != null)
                            rk.SetValue(appname, value, RegistryValueKind.String);
                        else
                            rk.DeleteValue(appname);
                    }
                }
            }
        }
    }
}
