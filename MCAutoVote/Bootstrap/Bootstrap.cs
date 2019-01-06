using MCAutoVote.CLI;
using MCAutoVote.Voting;
using MCAutoVote.Web;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace MCAutoVote.Bootstrap
{
    public class Bootstrap
    {
        [STAThread]
        private static void Main(string[] args)
        {
            ExceptionHandler.Attach();
            
            //load all modules
            LoadModuleAttribute.LoadAll();
            
            //run console interface
            CLI.CLI.Init();
            CLI.CLI.Welcome();

            //run main loop
            new Loop()
                .Add(() => CLILoop.Update())
                .Add(() => VoteLoop.Update())
                .Add(() => Application.DoEvents())
                .Run();
        }
    }
}
