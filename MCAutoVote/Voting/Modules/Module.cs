using MCAutoVote.Interface;
using MCAutoVote.Utilities.Multithreading;
using MCAutoVote.Web;

namespace MCAutoVote.Voting.Modules
{
    public abstract class Module
    {
        public int ProjectId { get; }
        public Module(int projectId)
        {
            ProjectId = projectId;
        }

        public abstract void Vote(IContext context);

        public abstract string Name { get; }

        public override string ToString()
        {
            return string.Format("{0} [Project ID: {1}]", Name, ProjectId);
        }

        protected static class Utilities
        {
            private static string OAuthVKHost { get; } = "oauth.vk.com";

            public static void CheckVKUserAuth()
            {
                IBrowser b = ApplicationContext.Browser;
                if (b.Url.Host.ToLower() == OAuthVKHost)
                {
                    Text.WriteLine("Waiting user for authorization");
                    MultithreadingUtils.WaitWhile(() => b.Url.Host.ToLower() == OAuthVKHost, 60000, 2000);
                }
            }
        }
    }
}
