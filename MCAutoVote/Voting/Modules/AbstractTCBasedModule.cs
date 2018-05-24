using MCAutoVote.Interface;
using MCAutoVote.Utilities;
using MCAutoVote.Web;
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

        public override void Vote(string nickname)
        {
            int attempts = 0;
            start:
            if (++attempts > MaxLoops)
                throw new Exception("Too many loops. Is it an authirization bug?");

            Browser b = ApplicationContext.Instance.Container.Browser;
            b.Navigate(Url);
            b.WaitComplete();

            //=== open voting modal window
            Text.WriteLine("Opening modal");
            (b.Document.All.GetElementsByClass("openLoginModal")
                           .FirstOrDefault() ??
             b.Document.All.GetElementsByClass("openVoteModal")
                           .FirstOrDefault())
                           .InvokeMember("click");

            b.WaitComplete();

            Text.WriteLine("Performing modal checks");
            HtmlElement elem =
                b.Document.All
                          .GetElementsByClass("modalVkLogin")
                          .FirstOrDefault();

            b.WaitComplete();

            if (elem == null)
            {
                Text.WriteLine("Voting for {0}", nickname);
                b.Document.GetElementById("nick")
                          .InnerText = nickname;
                b.Document.All
                          .GetElementsByClass("voteBtn")
                          .Single()
                          .InvokeMember("click");

                Text.WriteLine("Validating");
                HtmlElement tooltip = null;
                FunctionalUtils.WaitWhile(() => (tooltip = b.Document.All.GetElementsByClass("tooltip-inner").FirstOrDefault()) == null, 5600, 800);
                if (tooltip.InnerText == "Сегодня Вы уже голосовали")
                    throw new AbortException("Already voted! ^.^");
            }
            else
            {
                Text.WriteLine("Authorizing");

                //manually navigating because button not seems to be working
                b.Navigate(new Uri(new Uri(b.DocumentUrl.GetLeftPart(UriPartial.Authority)), @"/accounts/vk/login/?process=login"));
                b.WaitComplete();

                Utilities.CheckVKUserAuth();

                goto start; //releasing the satan from hell
            }

            b.WaitComplete();
        }
    }
}
