using MCAutoVote.Interface;
using MCAutoVote.Voting.Modules;
using MCAutoVote.Properties;
using System;
using System.Threading;
using MCAutoVote.Utilities.Sync;
using MCAutoVote.Bootstrap;
using System.Collections.Generic;
using MCAutoVote.Utilities;

namespace MCAutoVote.Voting
{
    public static class Vote
    {
        [LoadModule]
        public static class Auto
        {
            static Auto()
            {
                UpdateTimerState();
            }

            public static TimeSpan AutovotingDelay { get; } = TimeSpan.FromDays(1) + TimeSpan.FromMinutes(1);

            private static Timer timer = new Timer(TryVote, null, Timeout.Infinite, 30000);

            public static bool Enabled
            {
                get => Settings.Default.AutovoteEnabled;
                set
                {
                    Settings.Default.AutovoteEnabled = value;
                    UpdateTimerState();
                }
            }

            public static TimeSpan UntilAction => AutovotingDelay - (DateTime.UtcNow - LastAction);

            public static DateTime LastAction
            {
                get => Settings.Default.LastAction;
                set => Settings.Default.LastAction = value;
            }

            private static void UpdateTimerState()
            {
                timer.Change(Enabled ? 30000 : Timeout.Infinite, 30000);
            }

            private static void TryVote(object state)
            {
                if (!IsNicknameValid)
                    return;
                else if (actionLock.IsLocked)
                    return;
                else if (DateTime.UtcNow - LastAction < AutovotingDelay)
                    return;

                using (InterfaceLifecycle.OperationMutex.Use()) Do();
                LastAction = DateTime.UtcNow;
            }
        }

        public static int Attempts { get; } = 4;

        public static string Nickname
        {
            get => Settings.Default.Nickname;
            set => Settings.Default.Nickname = value;
        }

        public static bool IsNicknameValid => !StringUtils.IsNullEmptyOrWhitespace(Nickname);

        private static IEnumerable<Module> Modules { get; } = new HashSet<Module>(){
            new TopCraftModule(308),
            new MCRateModule(4396),
            new MCTopModule(1088)
        };

        private static ResourceLock actionLock = new ResourceLock();
        
        public static void Do()
        {
            if (!IsNicknameValid)
                throw new ArgumentException("Please set nickname for voting actions.");

            if (actionLock.IsLocked)
                throw new AbortException("Already in lock!");

            using (actionLock.Use())
            {
                Text.WriteLine("Nickname: {0}", ConsoleColor.Cyan, Settings.Default.Nickname);

                using (ApplicationContext.Instance.Container.CreateShowHandle())
                {
                    int success = 0;
                    int count = 0;

                    foreach(Module module in Modules)
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
                                module.Vote(Settings.Default.Nickname);
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

                                Anchor back = a.Set();
                                Text.Write("=> Abort: {0}", ConsoleColor.DarkYellow, e.Message);
                                back.Set();

                                Thread.Sleep(800);
                                break;
                            }
#if !DEBUG
                    catch (Exception e)
                    {
                        Text.Handler = DefaultTextHandler.Instance;

                        Anchor back = a.Set();
                        Text.Write(" => {0}: {1}", ConsoleColor.DarkRed, e.GetType().Name, e.Message);
                        back.Set();

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
            }
        }
    }
}
