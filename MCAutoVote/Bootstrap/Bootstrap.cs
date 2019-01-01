using MCAutoVote.CLI;
using MCAutoVote.Voting;
using MCAutoVote.Web;
using System;
using System.Threading;
using System.Windows.Forms;

namespace MCAutoVote.Bootstrap
{
    public class Bootstrap
    {
        private static Thread applicationThread;

        [STAThread]
        private static void Main(string[] args)
        {
            //load all modules
            LoadModuleAttribute.LoadAll();

            //run win32 application thread
            applicationThread = new Thread(() => Application.Run(ApplicationContext.Instance)) { IsBackground = true };
            applicationThread.SetApartmentState(ApartmentState.STA);
            applicationThread.Start();

            //run console interface
            CLI.CLI.Init();
            CLI.CLI.Welcome();

            //run main loop
            new Loop()
                .Add(() => CLILoop.Update())
                .Add(() => VoteLoop.Update())
                //.Add(() => Application.DoEvents())
                .Run();
        }
    }
}
