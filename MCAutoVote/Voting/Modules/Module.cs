using MCAutoVote.CLI;
using MCAutoVote.Utilities;
using MCAutoVote.Web;
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

        public abstract void Vote(string nickname);

        public abstract string Name { get; }

        public override string ToString()
        {
            return string.Format("{0} [Project ID: {1}]", Name, ProjectId);
        }

        protected static class Utilities
        {
            public static void CheckVKUserAuth()
            {
                Browser b = ApplicationContext.Instance.Container.Browser;
                if (b.DocumentUrl.Host.ToLower() == "oauth.vk.com")
                {
                    CLIOutput.WriteLine("Waiting user for authorization");
                    FunctionalUtils.WaitWhile(() => b.DocumentUrl.Host.ToLower() == "oauth.vk.com", 60000, 2000);
                }
            }
        }
    }
}
