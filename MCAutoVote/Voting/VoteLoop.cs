﻿using MCAutoVote.CLI;
using MCAutoVote.Voting.Modules;
using MCAutoVote.Properties;
using System;
using System.Threading;
using System.Collections.Generic;
using MCAutoVote.Utilities;

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

        public static VoteState State
        {
            get
            {
                if (!Enabled)
                    return VoteState.NotEnabled;
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
                case VoteState.Nickname:
                    throw new InvalidOperationException("Cannot vote: no nickname is set!");
            }

            PerformVote();
            LastVoteUtc = DateTime.UtcNow;
        }

        private static void PerformVote()
        {
            CLIOutput.WriteLine("Nickname: {0}", ConsoleColor.Cyan, Preferences.Preferences.Data.Nickname);

            int success = 0;
            int count = 0;

            foreach (Module module in Modules)
            {
                count++;
                for (int attempts = 0; attempts < Attempts; attempts++)
                {
                    try
                    {
                        CLIOutput.WriteLine("Voting: {0}. Attempt #{1}. ", ConsoleColor.White, module, attempts + 1);

                        module.Vote(Preferences.Preferences.Data.Nickname);
                        success++;

                        CLIOutput.WriteLine("Success!", ConsoleColor.Green);

                        break;
                    }
                    catch (AbortException e)
                    {
                        CLIOutput.WriteLine("Abort: {0}", ConsoleColor.DarkYellow, e.Message);

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

            CLIOutput.Write("Successful votes: {0}, ", ConsoleColor.Green, success);
            CLIOutput.WriteLine("failed: {0}", ConsoleColor.DarkRed, count - success);

            Thread.Sleep(100);
            CLIOutput.WriteLine("Completed!", ConsoleColor.White);
        }

        public static string StateString
        {
            get
            {
                switch(State)
                {
                    case VoteState.NotEnabled: return "Disabled!";
                    case VoteState.NoProblem: return "Can vote now!";
                    case VoteState.Nickname: return "No nickname is set!";
                    case VoteState.Time: return StringUtils.GetTimeString(TimeLeft) + " left";
                }

                throw new NotSupportedException();
            }
        }

        public enum VoteState
        {
            NoProblem,
            NotEnabled,
            Nickname,
            Time
        }
    }
}