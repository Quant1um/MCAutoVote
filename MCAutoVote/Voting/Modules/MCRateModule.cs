using MCAutoVote.Interface;
using MCAutoVote.Utilities;
using MCAutoVote.Web;
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
        public static TimeSpan LoopbackThreshold { get; } = TimeSpan.FromSeconds(25);
        public static int MaxLoops { get; } = 2;

        public MCRateModule(int projectId) : base(projectId) { }

        public override string Name => "mcrate.su";

        public override void Vote(string nickname)
        {
            int attempts = 0;
            start:
            if (++attempts > MaxLoops)
                throw new Exception("Too many loops! Is it a timer bug?");

            Browser b = ApplicationContext.Instance.Container.Browser;
            b.Navigate($"http://mcrate.su/project/{ProjectId}");
            b.WaitComplete();

            Text.WriteLine("Opening auth page");
            b.Document.All
                      .GetElementsByClass("fa-thumbs-o-up")
                      .First()
                      .InvokeMember("click");

            b.WaitComplete();

            Text.WriteLine("Authorizing");
            b.Document.All
                      .GetElementsByClass("vk_authorization")
                      .First()
                      .InvokeMember("click");

            b.WaitComplete();
            Utilities.CheckVKUserAuth();
            b.WaitComplete();

            Text.WriteLine("Performing checks");
            HtmlElement elem = b.Document.All
                                         .GetElementsByName("login_player")
                                         .Cast<HtmlElement>()
                                         .FirstOrDefault();

            if (elem == null)
            {
                TimeSpan span = TimeSpan.MinValue;
                try
                {
                    Thread.Sleep(1300);
                    HtmlElement timer = b.Document.All.GetElementsByClass("timer_count").Single();
                    int h = int.Parse(timer.Children.GetElementsByClass("count_hour").Single().InnerText);
                    int m = int.Parse(timer.Children.GetElementsByClass("count_min").Single().InnerText);
                    int s = int.Parse(timer.Children.GetElementsByClass("count_sec").Single().InnerText);
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
                

            Text.WriteLine("Voting for {0}", nickname);
            elem.InnerText = nickname;
            b.Document.GetElementById("buttonrate")
                      .InvokeMember("click");

            b.WaitComplete();
        }
    }
}
