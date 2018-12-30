using MCAutoVote.Bootstrap;
using MCAutoVote.CLI.Command;
using MCAutoVote.Properties;
using MCAutoVote.Utilities;
using MCAutoVote.Voting;
using System;
using System.Linq;

namespace MCAutoVote.CLI
{
    public static class CLI
    {
        public static Random Random { get; } = new Random();

        public static void HandleQuery(string query)
        {
            string[] splittedQuery = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedQuery.Length == 0) return;
            string command = splittedQuery[0].ToLower();

            Command.Command deleg = CommandRegistry.GetCommandByAlias(command);
            if (deleg != null)
#if !DEBUG
                    try
                    {
#endif
                deleg(query, splittedQuery.Skip(1).ToArray());
#if !DEBUG
                    }
                    catch (Exception e)
                    {
                        Anchor back = anchor.Set();
                        Text.Write("=> {0}: {1}", ConsoleColor.DarkRed, e.GetType().Name, e.Message);
                        back.Set();
                    }
#endif
            else
            {
                CLIOutput.Write("No such command!", ConsoleColor.DarkRed);

                int dist = int.MaxValue;
                string min = null;

                foreach (string alias in CommandRegistry.EnumerateAliases())
                {
                    int dd = StringUtils.EditDistance(command, alias);
                    if (dd < dist)
                    {
                        dist = dd;
                        min = alias;
                    }
                }

                if (dist <= 3 && min != null)
                    CLIOutput.WriteLine(" Did you mean '{0}'?", ConsoleColor.DarkRed, min);
                else
                    CLIOutput.WriteLine();
            }
        }

        public static void Init()
        {
            Console.Title = RandomSplash();
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Welcome()
        {
            CLIOutput.WriteLine("Welcome to {0}", Info.FullName);

            CLIOutput.WriteLine(@"Write 'help' to list all supported commands.");
            if (!Info.Autostart)
                CLIOutput.WriteLine("Autostart disabled! You can enable it using 'autostart enable' command, if you want.", ConsoleColor.DarkYellow);
            if (!VoteLoop.Enabled)
                CLIOutput.WriteLine("Autovote disabled! You can enable it using 'autovote enable' command, if you want.", ConsoleColor.DarkYellow);
        }

        private static string RandomSplash()
        {
            string[] splashes = Resources.Splashes.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return splashes[new Random().Next(splashes.Length)];
        }
    }
}
