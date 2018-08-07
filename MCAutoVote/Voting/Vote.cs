using MCAutoVote.Interface;
using MCAutoVote.Voting.Modules;
using System;
using System.Threading;
using System.Collections.Generic;
using MCAutoVote.Web;
using MCAutoVote.Utilities.Persistency;

namespace MCAutoVote.Voting
{
    public static class Vote
    {
        public static int Attempts { get; } = 4;

        private class Context : IContext
        {
            public static Context Instance { get; } = new Context();

            private Context() { }

            public string Nickname => Vote.Nickname;
            public IBrowser Browser => ApplicationContext.Browser;
        }

        public static TimeSpan AutovotingDelay { get; } = TimeSpan.FromDays(1) + TimeSpan.FromMinutes(1);
        public static TimeSpan UntilAction => AutovotingDelay - (DateTime.UtcNow - LastAction);

        public static DateTime LastAction
        {
            get => State.LastAction;
            set => State.LastAction = value;
        }

        public static string Nickname
        {
            get => Preferences.Nickname;
            set => Preferences.Nickname = value;
        }

        public static bool IsNicknameValid => !string.IsNullOrWhiteSpace(Nickname);

        private static IEnumerable<Module> Modules { get; } = new HashSet<Module>(){
            new TopCraftModule(308),
            new MCRateModule(4396),
            new MCTopModule(1088)
        };

        public static void Do()
        {
            if (!IsNicknameValid)
                throw new ArgumentException("Please set valid nickname for voting.");

            Text.WriteLine("Nickname: {0}", ConsoleColor.Cyan, Nickname);

            using (ApplicationContext.Instance.Container.CreateShowHandle())
            {
                int success = 0;
                int count = 0;

                foreach (Module module in Modules)
                {
                    count++;
                    for (int attempts = 0; attempts < Attempts; attempts++)
                    {
                        Anchor a = null;
                        try
                        {
                            Text.Write("Voting: {0}. Attempt #{1}. ", ConsoleColor.White, module, attempts + 1);
                            a = new Anchor();
                            Text.WriteLine();

                            Text.Handler = ModuleTextHandler.Instance;
                            module.Vote(Context.Instance);
                            Text.Handler = DefaultTextHandler.Instance;

                            success++;

                            Anchor back = a.Set();
                            Text.Write("=> Success!", ConsoleColor.Green);
                            back.Set();

                            break;
                        }
                        catch (AbortException e)
                        {
                            Text.Handler = DefaultTextHandler.Instance;

                            using (a.Use()) 
                                Text.Write("=> Abort: {0}", ConsoleColor.DarkYellow, e.Message);

                            Thread.Sleep(800);
                            break;
                        }
#if !DEBUG
                        catch (Exception e)
                        {
                            Text.Handler = DefaultTextHandler.Instance;

                            using (a.Use()) 
                                Text.Write(" => {0}: {1}", ConsoleColor.DarkRed, e.GetType().Name, e.Message);

                            Thread.Sleep(800);
                        }
#endif
                    }
                }

                Text.Write("Successful votings: {0}, ", ConsoleColor.Green, success);
                Text.WriteLine("failed: {0}", ConsoleColor.DarkRed, count - success);
            }

            Thread.Sleep(100);
            Text.WriteLine("Completed!", ConsoleColor.White);

            LastAction = DateTime.UtcNow;
        }
    }
}
