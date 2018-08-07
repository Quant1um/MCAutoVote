using MCAutoVote.Interface;
using MCAutoVote.Utilities.Multithreading;
using MCAutoVote.Web;
using MSHTML;
using System;

namespace MCAutoVote.Voting.Modules
{
    public abstract class AbstractTCBasedModule : Module
    {
        public static int MaxLoops { get; } = 4;

        public AbstractTCBasedModule(int projectId) : base(projectId) { }

        protected abstract string Url { get; }

        public override void Vote(IContext context)
        {
            int attempts = 0;
            start:
            if (++attempts > MaxLoops)
                throw new Exception("Too many loops. Is it an authirization bug?");

            IBrowser b = context.Browser;
            b.Navigate(Url);
            b.WaitComplete();

            //=== open voting modal window
            Text.WriteLine("Opening modal");
            (b.QuerySelector(".openLoginModal") ??
             b.QuerySelector(".openVoteModal"))
                           .click();

            b.WaitComplete();

            Text.WriteLine("Performing modal checks");
            IHTMLElement elem = b.QuerySelector(".modalVkLogin");

            b.WaitComplete();

            if (elem == null)
            {
                Text.WriteLine("Voting for {0}", context.Nickname);
                b.QuerySelector("#nick").innerText =  context.Nickname;
                b.QuerySelector(".voteBtn").click();

                Text.WriteLine("Validating");
                IHTMLElement tooltip = null;
                MultithreadingUtils.WaitWhile(() => (tooltip = b.QuerySelector(".tooltip-inner")) == null, 5600, 800);
                if (tooltip.innerText == "Сегодня Вы уже голосовали")
                    throw new AbortException("Already voted! ^.^");
            }
            else
            {
                Text.WriteLine("Authorizing");

                //manually navigating because button not seems to be working
                b.Navigate(new Uri(new Uri(b.Url.GetLeftPart(UriPartial.Authority)), @"/accounts/vk/login/?process=login"));
                b.WaitComplete();

                Utilities.CheckVKUserAuth();

                goto start; //releasing the satan from hell
            }

            b.WaitComplete();
        }
    }
}
