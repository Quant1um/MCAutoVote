using MCAutoVote.Bootstrap;
using MCAutoVote.Interface.CommandControl;
using MCAutoVote.Properties;
using MCAutoVote.Utilities;
using MCAutoVote.Voting;
using System;
using System.Threading;

namespace MCAutoVote.Interface
{
    public static class CommandLineInterface
    {
        private static bool running = false;
        public static void Run()
        {
            if (running)
                throw new InvalidOperationException("CLI already running!");
            running = true;

            Setup();
            Welcome();

            while (true)
            {
                Anchor anchor = new Anchor();
                Text.Write("> ", ConsoleColor.Green);
                string query = ConsoleWindow.ReadLine();

                if (string.IsNullOrWhiteSpace(query))
                {
                    anchor.Set();
                    continue;
                }

                anchor.Set();
                Text.Write("> ", ConsoleColor.Gray);
                Text.Write(query + " ");
                anchor = new Anchor();
                Text.WriteLine();

                Arguments args = new Arguments(query);
                Command deleg = CommandRegistry.GetCommandByAlias(args.Command);
                if (deleg != null)
                {
#if !DEBUG
                    try
                    {
#endif
                    deleg(args);
#if !DEBUG
                    }
                    catch (Exception e)
                    {
                        Anchor back = anchor.Set();
                        Text.Write("=> {0}: {1}", ConsoleColor.DarkRed, e.GetType().Name, e.Message);
                        back.Set();
                    }
#endif
                }
                else
                {
                    using (anchor.Use())
                    {
                        Text.Write("=> No such command!", ConsoleColor.DarkRed);

                        int dist = int.MaxValue;
                        string min = null;

                        foreach (string alias in CommandRegistry.EnumerateAliases())
                        {
                            int dd = StringUtils.EditDistance(args.Command, alias);
                            if (dd < dist)
                            {
                                dist = dd;
                                min = alias;
                            }
                        }

                        if (dist <= 3 && min != null)
                            Text.Write(" Did you mean '{0}'?", ConsoleColor.DarkRed, min);
                    }
                }
            }
        }

        private static void Setup()
        {
            ConsoleWindow.Title = RandomSplash();
            ConsoleWindow.Foreground = ConsoleColor.White;

            ApplicationContext.Tray.DoubleClick += () => ConsoleWindow.Hidden = !ConsoleWindow.Hidden;
        }

        private static void Welcome()
        {
            Text.WriteLine("Welcome to {0}", Info.FullName);

            Text.WriteLine(@"Write 'help' to list all supported commands.");
            if (!Info.Autostart)
                Text.WriteLine("Autostart disabled! You can enable it using 'autostart enable' command, if you want.", ConsoleColor.DarkYellow);
            if (!Vote.IsNicknameValid)
                Text.WriteLine("Nickname is not set! You can set it using 'nickname <nickname>' command, if you want.", ConsoleColor.DarkYellow);
        }

        private static string RandomSplash()
        {
            string[] splashes = Resources.Splashes.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return splashes[new Random().Next(splashes.Length)];
        }
    }
}
