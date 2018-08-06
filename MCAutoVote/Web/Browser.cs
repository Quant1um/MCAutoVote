using MCAutoVote.Bootstrap;
using MCAutoVote.Utilities;
using MCAutoVote.Utilities.Multithreading;
using MSHTML;
using System;
using System.Threading;
using System.Windows.Forms;

namespace MCAutoVote.Web
{
    [LoadModule]
    public class Browser : WebBrowser, IBrowser
    {
        static Browser()
        {
            RegistryUtils.BrowserEmulation[Info.ExecutableName] = true;
        }

        internal Browser()
        {
            ScriptErrorsSuppressed = true;
            Dock = DockStyle.Fill;
            AllowWebBrowserDrop = false;
            IsWebBrowserContextMenuEnabled = false;
        }

        #region Multithreading

        private void Do(Action a)
        {
            if (InvokeRequired)
                BeginInvoke(new MethodInvoker(a));
            else a();
        }

        private T Do<T>(Func<T> a)
        {
            if (InvokeRequired)
                return (T)Invoke(a);
            else return a();
        }

        #endregion

        private HTMLDocument MSHTMLDocument => (HTMLDocument)Do(() => Document.DomDocument);

        public IHTMLElement QuerySelector(string selector) =>
            MSHTMLDocument.querySelector(selector);

        public IHTMLDOMChildrenCollection QuerySelectorAll(string selector) =>
            MSHTMLDocument.querySelectorAll(selector);

        public bool Completed
        {
            get
            {
                if (Do(() => IsBusy)) return false;

                switch (Do(() => ReadyState))
                {
                    case WebBrowserReadyState.Complete:
                    case WebBrowserReadyState.Interactive:
                        return true;
                    default: return false;
                }
            }
        }

        public void WaitComplete(long timeout = long.MaxValue)
        {
            Thread.Sleep(400);
            MultithreadingUtils.WaitWhile(() => !Completed, timeout, 400);
        }
    }
}
