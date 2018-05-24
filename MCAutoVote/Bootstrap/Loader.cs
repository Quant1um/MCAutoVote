using MCAutoVote.Interface;
using MCAutoVote.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace MCAutoVote.Bootstrap
{
    public class Loader
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
            InterfaceLifecycle.Run();
        }
    }
}
