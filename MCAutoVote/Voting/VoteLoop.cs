using MCAutoVote.CLI;
using MCAutoVote.Voting.Modules;
using MCAutoVote.Properties;
using System;
using System.Threading;
using System.Collections.Generic;
using MCAutoVote.Utilities;
using OpenQA.Selenium.Remote;
using MCAutoVote.Web;
using OpenQA.Selenium;

namespace MCAutoVote.Voting
{
    public static class VoteLoop
    {
        public static int Attempts { get; } = 4;

        public static TimeSpan Delay { get; } = TimeSpan.FromDays(1) + TimeSpan.FromSeconds(16);

        public static DateTime LastVoteUtc
        {
            get => Preferences.Preferences.Data.LastVote;
            set => Preferences.Preferences.Data.LastVote = value;
        }

        public static TimeSpan TimeLeft => LastVoteUtc + Delay - DateTime.UtcNow;

        public static bool Enabled
        {
            get => Preferences.Preferences.Data.Enabled;
            set => Preferences.Preferences.Data.Enabled = value;
        }

        public static string Nickname
        {
            get => Preferences.Preferences.Data.Nickname;
            set => Preferences.Preferences.Data.Nickname = value;
        }

        public static bool LastTimeFailed
        {
            get => Preferences.Preferences.Data.LastTimeFailed;
            set => Preferences.Preferences.Data.LastTimeFailed = value;
        }

        public static WebDriverInfo DriverInfo => Preferences.Preferences.Data.DriverInfo;

        public static VoteState State
        {
            get
            {
                if (LastTimeFailed)
                    return VoteState.LastTimeFailed;
                if (!Enabled)
                    return VoteState.NotEnabled;
                if (!DriverInfo.IsWebDriverSupported)
                    return VoteState.BrowserSetting;
                if (StringUtils.IsNullEmptyOrWhitespace(Nickname))
                    return VoteState.Nickname;
                if (TimeLeft > TimeSpan.Zero)
                    return VoteState.Time;
                return VoteState.NoProblem;
            }
        }

        private static IEnumerable<Module> Modules { get; } = new HashSet<Module>(){
            new TopCraftModule(308),
            new MCRateModule(4396),
            new MCTopModule(1088)
        };

        public static void Update()
        {
            if (State == VoteState.NoProblem)
            {
                Vote();
            }
        }

        public static void Vote()
        {
            switch(State)
            {
                case VoteState.BrowserSetting:
                    throw new InvalidOperationException("Cannot vote: no browser is set!");
                case VoteState.Nickname:
                    throw new InvalidOperationException("Cannot vote: no nickname is set!");
            }

            try
            {
                PerformVote();
                LastTimeFailed = false;
                LastVoteUtc = DateTime.UtcNow;
            } catch(Exception e) {
                LastTimeFailed = true;
                CLIOutput.WriteLine("Unknown error: {0} - {1}", ConsoleColor.DarkRed, e.GetType().Name, e.Message);
            }
        }

        private static void PerformVote()
        {
            try
            {

                CLIOutput.WriteLine("Nickname: {0}", ConsoleColor.Cyan, Preferences.Preferences.Data.Nickname);

                int success = 0;
                int count = 0;

                using (VoteContext ctx = VoteContext.Create())
                {
                    foreach (Module module in Modules)
                    {
                        count++;
                        for (int attempts = 0; attempts < Attempts; attempts++)
                        {
                            try
                            {
                                CLIOutput.WriteLine("Voting: {0}. Attempt #{1}. ", ConsoleColor.White, module, attempts + 1);

                                module.Vote(ctx);
                                success++;

                                CLIOutput.WriteLine("Success!", ConsoleColor.Green);

                                break;
                            }
                            catch (NoSuchWindowException e)
                            {
                                CLIOutput.WriteLine("Window was closed: voting has been terminated", ConsoleColor.DarkYellow);
                                CLIOutput.WriteLine(e.Message);

                                Thread.Sleep(800);
                                return;
                            }
                            catch (InvalidOperationException e)
                            {
                                CLIOutput.WriteLine("Session lost.", ConsoleColor.DarkYellow);
                                CLIOutput.WriteLine(e.Message);
                                
                                Thread.Sleep(800);
                                return;
                            }
                            catch (AbortException e)
                            {
                                CLIOutput.WriteLine("Aborted: {0}", ConsoleColor.DarkYellow, e.Message);

                                Thread.Sleep(800);
                                break;
                            }
#if !DEBUG_MODULES
                            catch (Exception e)
                            {
                                CLIOutput.WriteLine("Error: {0} - {1}", ConsoleColor.DarkRed, e.GetType().Name, e.Message);
                                Thread.Sleep(800);
                            }
#endif
                        }
                    }
                }

                CLIOutput.Write("Successful votes: {0}, ", ConsoleColor.Green, success);
                CLIOutput.WriteLine("failed: {0}", ConsoleColor.DarkRed, count - success);

                Thread.Sleep(100);
                CLIOutput.WriteLine("Completed!", ConsoleColor.White);
            }
            catch (NoSuchWindowException e)
            {
                CLIOutput.WriteLine("Window was closed: voting has been terminated", ConsoleColor.DarkYellow);
                CLIOutput.WriteLine(e.Message);

                Thread.Sleep(800);
                return;
            }
            catch (InvalidOperationException e)
            {
                CLIOutput.WriteLine("Session was failed to create.", ConsoleColor.DarkYellow);
                CLIOutput.WriteLine(e.Message);

                Thread.Sleep(800);
                return;
            }
        }

        public static string StateString
        {
            get
            {
                switch(State)
                {
                    case VoteState.LastTimeFailed: return "Suspended due to unknown failure: run 'vote' command to restore";
                    case VoteState.NotEnabled: return "Disabled!";
                    case VoteState.BrowserSetting: return "Browser isn't set!";
                    case VoteState.NoProblem: return "Voting...";
                    case VoteState.Nickname: return "Nickname isn't set!";
                    case VoteState.Time: return StringUtils.GetTimeString(TimeLeft) + " left";
                }

                throw new NotSupportedException();
            }
        }

        public enum VoteState
        {
            NoProblem,
            LastTimeFailed,
            BrowserSetting,
            NotEnabled,
            Nickname,
            Time
        }

        private class VoteContext : IVoteContext, IDisposable
        {
            public static VoteContext Create() => new VoteContext();

            private readonly WebDriverWrapper wrapper;

            public string Nickname { get; }
            public RemoteWebDriver Driver => wrapper.Driver;

            private VoteContext()
            {
                Nickname = VoteLoop.Nickname;

                wrapper = DriverInfo.CreateWrapper();
            }
            
            public void Log(string str, params object[] parameters)
            {
                CLIOutput.WriteLine("| " + str, ConsoleColor.Gray, parameters);
            }

            public void Dispose()
            {
                wrapper.Dispose();
            }
        }
    }
}
