using Internal.ReadLine;
using System;

namespace MCAutoVote.CLI
{
    public static class CLILoop
    {
        private static KeyHandler handler = ReadLine.CreateHandler();

        public static void Update()
        {
            if(Console.KeyAvailable)
            {
                HandleKey(Console.ReadKey(true));
            }
        }

        private static void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key != ConsoleKey.Enter) {
                handler.Handle(keyInfo);
            } else {
                CLIOutput.WriteLine();
                CLI.HandleQuery(handler.Text);
                ReadLine.AddHistory(handler.Text);
                handler = ReadLine.CreateHandler();
            }
        }
    }
}
