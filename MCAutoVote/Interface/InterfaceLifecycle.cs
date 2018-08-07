using MCAutoVote.Bootstrap;
using MCAutoVote.Interface.CommandControl;
using MCAutoVote.Properties;
using MCAutoVote.Utilities;
using MCAutoVote.Utilities.Persistency;
using MCAutoVote.Voting;
using System;
using System.Linq;
using static MCAutoVote.NativeMethods;

namespace MCAutoVote.Interface
{
    [LoadModule]
    public static class InterfaceLifecycle
    {
        static InterfaceLifecycle()
        {
            UpdateConsoleState();
        }

        public static Random Random { get; } = new Random();

        public static bool ConsoleHidden
        {
            get => Preferences.Hidden;
            set {
                Preferences.Hidden = value;
                UpdateConsoleState();
            }
        }

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
                string query = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(query))
                {
                    anchor.Set();
                    continue;
                }

                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Text.Write("> ", ConsoleColor.Gray);
                Text.Write(query + " ");
                anchor = new Anchor();
                Text.WriteLine();

                string[] splittedQuery = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (splittedQuery.Length == 0) continue;
                string command = splittedQuery[0].ToLower();

                Command deleg = CommandRegistry.GetCommandByAlias(command);
                if (deleg != null)
                {
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
                            int dd = StringUtils.EditDistance(command, alias);
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
            UpdateConsoleState();
            Console.Title = RandomSplash();
            Console.ForegroundColor = ConsoleColor.White;

            ApplicationContext.Tray.DoubleClick += () => ConsoleHidden = !ConsoleHidden;
        }

        private static void UpdateConsoleState() => ShowWindow(GetConsoleWindow(), ConsoleHidden ? SW_HIDE : SW_SHOW);

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
