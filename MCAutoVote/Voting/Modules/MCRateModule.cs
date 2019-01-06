using MCAutoVote.CLI;
using MCAutoVote.Utilities;
using MCAutoVote.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MCAutoVote.Voting.Modules
{
    public class MCRateModule : Module
    {
        public static TimeSpan LoopbackThreshold { get; } = TimeSpan.FromSeconds(3);
        public static int MaxLoops { get; } = 2;

        public MCRateModule(int projectId) : base(projectId) { }

        public override string Name => "mcrate.su";

        public override void Vote(IVoteContext ctx)
        {
            int attempts = 0;
            start:
            if (++attempts > MaxLoops)
                throw new Exception("Too many loops! Is it a timer bug?");

            RemoteWebDriver driver = ctx.Driver;

            driver.Url = $"http://mcrate.su/project/{ProjectId}";

            ctx.Log("Opening auth page");
            driver.FindElement(By.CssSelector(".fa-thumbs-o-up")).Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            ctx.Log("Authorizing");
            driver.FindElement(By.CssSelector(".vk_authorization")).Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            Utilities.CheckVKUserAuth(ctx);

            ctx.Log("Performing checks");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            IWebElement elem = driver.FindElements(By.Name("login_player")).FirstOrDefault();
            if (elem == null)
            {
                TimeSpan span = TimeSpan.MinValue;
                try
                {
                    Thread.Sleep(1300);
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                    int h = int.Parse(driver.FindElement(By.CssSelector(".timer_count .count_hour")).Text);
                    int m = int.Parse(driver.FindElement(By.CssSelector(".timer_count .count_min")).Text);
                    int s = int.Parse(driver.FindElement(By.CssSelector(".timer_count .count_sec")).Text);
                    span = new TimeSpan(h, m, s);
                }
                catch (Exception) { }

                if (span < LoopbackThreshold)
                    goto start; //...

                if(span.Ticks <= 0)
                    throw new AbortException("Already voted! ^.^");
                else
                    throw new AbortException("Already voted! ^.^ Please wait for " + StringUtils.GetTimeString(span) + ".");
            }


            ctx.Log("Voting for {0}", ctx.Nickname);
            elem.SendKeys(ctx.Nickname);

            driver.FindElement(By.CssSelector("#buttonrate")).Click();

            Thread.Sleep(5000); //TODO replace
        }
    }
}
