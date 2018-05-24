using ScrapySharp.Network;
using System.Net;

namespace MCAutoVote
{
    public static class Browser
    {
        public static ScrapingBrowser Scraping { get; } = new ScrapingBrowser
        {
            UserAgent = FakeUserAgents.Chrome,
            AllowMetaRedirect = true,
            AllowAutoRedirect = true,
            UseDefaultCookiesParser = false,
            DecompressionMethods = DecompressionMethods.GZip
        };

        static Browser()
        {
            LoadCookies();
        }

        public static void LoadCookies()
        {

        }

        public static void SaveCookies()
        {
            
        }
    }
}
