using MCAutoVote.CLI;
using MCAutoVote.Utilities;
using MCAutoVote.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace MCAutoVote.Voting.Modules
{
    public abstract class Module
    {
        public int ProjectId { get; }
        public Module(int projectId)
        {
            ProjectId = projectId;
        }

        public abstract void Vote(IVoteContext context);

        public abstract string Name { get; }

        public override string ToString()
        {
            return string.Format("{0} [Project ID: {1}]", Name, ProjectId);
        }

        protected static class Utilities
        {
            public static void CheckVKUserAuth(IVoteContext ctx)
            {
                ctx.Log("Waiting user for authorization");
                new WebDriverWait(ctx.Driver, TimeSpan.FromMinutes(1.2f))
                    .Until((d) => new Uri(d.Url).Host.ToLower() != "oauth.vk.com");
            }
        }
    }
}
