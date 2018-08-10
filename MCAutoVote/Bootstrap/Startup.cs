using MCAutoVote.Interface;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace MCAutoVote.Bootstrap
{
    /* TODO:
     *  - command queue (or 'waiter' command) [+]
     *  - settings (improve) [+]
     *  - autostart (improve)
     *  - refactor logging
     *  - new icon
     *  - new splashes
     *  - refactor cli
     *  
     *  - refactor:
     *      - InterfaceLifecycle
     *      - Vote
     *      - ApplicationContext
     */
    public class Startup
    {
        [STAThread]
        private static void Main(string[] args)
        {
            //setting culture to invariant
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            //load all modules
            LoadModuleAttribute.LoadAll();

            //run win32 application thread
            Thread applicationThread = new Thread(() => Application.Run(ApplicationContext.Instance)) { IsBackground = true };
            applicationThread.SetApartmentState(ApartmentState.STA);
            applicationThread.Start();

            //run console interface
            CommandLineInterface.Run();
        }
    }
}
