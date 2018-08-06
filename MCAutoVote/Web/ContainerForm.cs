using MCAutoVote.Utilities.Multithreading;
using System;
using System.Windows.Forms;

namespace MCAutoVote.Web
{
    public class ContainerForm : Form
    {
        private readonly Browser browser;
        private readonly ResourceLock showLock;

        public IBrowser Browser => browser;

        internal ContainerForm()
        {
            SuspendLayout();
            Controls.Add(browser = new Browser());
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
            if (e.CloseReason == CloseReason.UserClosing ||
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
