using MCAutoVote.Interface;
using MCAutoVote.Utilities;
using MCAutoVote.Web;
using MSHTML;
using System;
using System.Threading;

namespace MCAutoVote.Voting.Modules
{
    public class MCRateModule : Module
    {
        public static TimeSpan LoopbackThreshold { get; } = TimeSpan.FromSeconds(25);
        public static int MaxLoops { get; } = 2;

        public MCRateModule(int projectId) : base(projectId) { }

        public override string Name => "mcrate.su";

        public override void Vote(IContext context)
        {
            int attempts = 0;
            start:
            if (++attempts > MaxLoops)
                throw new InvalidProgramException("Too many loops! Is it a timer bug?");

            IBrowser b = context.Browser;
            b.Navigate($"http://mcrate.su/project/{ProjectId}");
            b.WaitComplete();

            Text.WriteLine("Opening auth page");
            b.QuerySelector(".fa-thumbs-o-up").click();

            b.WaitComplete();

            Text.WriteLine("Authorizing");
            b.QuerySelector(".vk_authorization").click();

            b.WaitComplete();
            Utilities.CheckVKUserAuth();
            b.WaitComplete();

            Text.WriteLine("Performing checks");
            IHTMLElement elem = b.QuerySelector("[name=\"login_player\"]");

            if (elem == null)
            {
                TimeSpan span = TimeSpan.MinValue;
                try
                {
                    Thread.Sleep(1300);
                    int h = int.Parse(b.QuerySelector(".timer_count .count_hour").innerText);
                    int m = int.Parse(b.QuerySelector(".timer_count .count_min").innerText);
                    int s = int.Parse(b.QuerySelector(".timer_count .count_sec").innerText);
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
                

            Text.WriteLine("Voting for {0}", context.Nickname);

            elem.innerText = context.Nickname;
            b.QuerySelector("#buttonrate").click();

            b.WaitComplete();
        }
    }
}
