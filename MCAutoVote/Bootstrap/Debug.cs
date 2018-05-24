using MCAutoVote.Interface;
using System;
using System.IO;
using System.Text;

namespace MCAutoVote.Bootstrap
{
    [LoadModule]
    public static class Debug
    {
        static Debug()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private static bool _exceptionCallbackExecuted = false;
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (_exceptionCallbackExecuted) return;
            _exceptionCallbackExecuted = true;

            if (e.ExceptionObject is Exception ex)
                Text.WriteLine("Unhandled exception => {0}", ConsoleColor.DarkRed, ex.Message);
            else if (e.ExceptionObject != null)
                Text.WriteLine("Unhandled error => {0}", ConsoleColor.DarkRed, e.ExceptionObject);
            else
                Text.WriteLine("Undefined error was occurred!", ConsoleColor.DarkRed);
            
            WriteErrorToFile(e.ExceptionObject);
        }

        private static void WriteErrorToFile(object ex)
        {
            StringBuilder content = new StringBuilder();
            if (ex == null)
                content.Append("Something went wrong... Exception object is null!");
            else
            {
                content.AppendLine("Type: " + ex.GetType().FullName);
                content.Append(ex.ToString());
            }

            string path = Path.Combine(Info.Directory, "stacktrace.log");
            File.WriteAllText(path, content.ToString());
            Text.WriteLine("Error info has been saved to {0}", ConsoleColor.Gray, path);
        }

    }
}
