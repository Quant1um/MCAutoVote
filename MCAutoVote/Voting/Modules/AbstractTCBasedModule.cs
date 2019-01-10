using MCAutoVote.CLI;
using MCAutoVote.Utilities;
using MCAutoVote.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MCAutoVote.Voting.Modules
{
    public abstract class AbstractTCBasedModule : Module
    {
        public static int MaxLoops { get; } = 4;

        public AbstractTCBasedModule(int projectId) : base(projectId) { }

        protected abstract string Url { get; }

        public override void Vote(IVoteContext ctx)
        {
            int attempts = 0;
            start:
            if (++attempts > MaxLoops)
                throw new Exception("Too many loops. Is it an authorization bug?");

            RemoteWebDriver driver = ctx.Driver;

            driver.Url = Url;

            //=== open voting modal window
            ctx.Log("Opening modal");
            
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            driver.FindElement(By.CssSelector(".openLoginModal, .openVoteModal")).Click();

            ctx.Log("Performing modal checks");

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            IWebElement elem = driver.FindElements(By.CssSelector(".modalVkLogin")).FirstOrDefault();

            if (elem == null)
            {
                ctx.Log("Voting for {0}", ctx.Nickname);

                driver.FindElement(By.CssSelector("#nick")).Clear();
                driver.FindElement(By.CssSelector("#nick")).SendKeys(ctx.Nickname);
                driver.FindElement(By.CssSelector(".voteBtn")).Click();

                ctx.Log("Validating");

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                if (driver.FindElement(By.CssSelector(".tooltip-inner")).Text == "Сегодня Вы уже голосовали")
                    throw new AbortException("Already voted! ^.^");
            }
            else
            {
                ctx.Log("Authorizing");

                //manually navigating because button not seems to be working
                driver.Url = new Uri(new Uri(new Uri(driver.Url).GetLeftPart(UriPartial.Authority)), @"/accounts/vk/login/?process=login").ToString();
                Utilities.CheckVKUserAuth(ctx);

                goto start; //releasing the satan from hell
            }
        }
    }
}
