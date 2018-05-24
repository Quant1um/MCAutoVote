using MCAutoVote.Bootstrap;
using MCAutoVote.Utilities;
using MCAutoVote.Utilities.Sync;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MCAutoVote.Web
{
    [LoadModule]
    public class Browser : WebBrowser
    {
        static Browser()
        {
            RegistryUtils.BrowserEmulation[Info.ExecutableName] = true;
        }

        private Browser()
        {
            ScriptErrorsSuppressed = true;
            Dock = DockStyle.Fill;
            AllowWebBrowserDrop = false;
            IsWebBrowserContextMenuEnabled = false;
        }

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

        private bool Completed
        {
            get
            {
                if (base.IsBusy) return false;

                switch (base.ReadyState)
                {
                    case WebBrowserReadyState.Complete:
                    case WebBrowserReadyState.Interactive:
                        return true;
                    default: return false;
                }
            }
        }

        public Uri DocumentUrl => Do(() => Document.Url);
        public new bool IsBusy => Do(() => base.IsBusy);
        public new WebBrowserReadyState ReadyState => Do(() => base.ReadyState);
        public new void Navigate(string url) => Do(() => base.Navigate(url));
        public new HtmlDocument Document => Do(() => base.Document);

        public void WaitComplete()
        {
            if (InvokeRequired)
                FunctionalUtils.WaitWhile(() => Do(() => !Completed), 30000, 500);
            else
                throw new InvalidOperationException();
        }

        public class ContainerForm : Form
        {
            public Browser Browser { get; }
            private ResourceLock showLock;

            internal ContainerForm() {
                SuspendLayout();
                Controls.Add(Browser = new Browser());
                ResumeLayout(false);
                PerformLayout();
                base.Hide();

                showLock = new ResourceLock(Show, Hide);
            }

            private bool loaded = false;
            //https://stackoverflow.com/questions/2041251/c-sharp-how-to-enable-form-by-double-clicking-on-tray-icon
            protected override void SetVisibleCore(bool value)
            {
                if (value && !loaded)
                {
                    CreateHandle();
                    value = false;
                    loaded = true;
                }
                base.SetVisibleCore(value);
            }

            protected override void OnFormClosing(FormClosingEventArgs e)
            {
                base.OnFormClosing(e);
                if(e.CloseReason == CloseReason.UserClosing || 
                   e.CloseReason == CloseReason.TaskManagerClosing ||
                   e.CloseReason == CloseReason.FormOwnerClosing ||
                   e.CloseReason == CloseReason.MdiFormClosing)
                    e.Cancel = true;
            }

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

            private new void Show() => Do(() => base.Show());
            private new void Hide() => Do(() => base.Hide());

            public IDisposable CreateShowHandle() => showLock.Use();
        }
    }
}
