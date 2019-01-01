using Internal.ReadLine;
using MCAutoVote.Voting;
using System;

namespace MCAutoVote.CLI
{
    public static class CLILoop
    {
        private static KeyHandler handler = null;

        public static void Update()
        {
            if(handler == null)
            {
                CreateHandler();
            }

            if(Console.KeyAvailable)
            {
                HandleKey(Console.ReadKey(true));
            }

            CLI.UpdateTitle();
        }

        private static void CreateHandler()
        {
            handler = ReadLine.CreateHandler();
            CLIOutput.Write("> ", ConsoleColor.Green);
        }

        private static void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key != ConsoleKey.Enter) {
                handler.Handle(keyInfo);
            } else {
                CLIOutput.WriteLine();
                CLI.HandleQuery(handler.Text);
                ReadLine.AddHistory(handler.Text);
                CreateHandler();
            }
        }
    }
}
