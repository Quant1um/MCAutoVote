using MCAutoVote.Bootstrap;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using static MCAutoVote.Web.Browser;

namespace MCAutoVote
{
    public class ApplicationContext : System.Windows.Forms.ApplicationContext
    {
        public static ApplicationContext Instance { get; } = new ApplicationContext();

        public TrayManager Tray { get; }
        public ContainerForm Container { get; } 
        
        private ApplicationContext() {
            MainForm = Container = new ContainerForm();
            Tray = new TrayManager();
        }

        public class TrayManager : IDisposable
        {
            private NotifyIcon icon;

            public event Action BalloonClick;
            public event Action DoubleClick;
            public event Action Click;

            internal TrayManager()
            {
                icon = new NotifyIcon()
                {
                    Text = Info.Name,
                    BalloonTipTitle = Info.Name,
                    Icon = Icon.ExtractAssociatedIcon(Info.ExecutablePath),
                    Visible = true
                };

                icon.Click += OnClicked;
                icon.BalloonTipClicked += OnBalloonClicked;
                icon.DoubleClick += OnDoubleClicked;
            }

            private void OnClicked(object sender, EventArgs e) => Click?.Invoke();
            private void OnBalloonClicked(object sender, EventArgs e) => BalloonClick?.Invoke();
            private void OnDoubleClicked(object sender, EventArgs e) => DoubleClick?.Invoke();

            public void Bubble(string message, ToolTipIcon icon = ToolTipIcon.None)
            {
                if (disposed)
                    throw new ObjectDisposedException("TrayManager");

                this.icon.BalloonTipIcon = icon;
                this.icon.BalloonTipText = message;
                this.icon.ShowBalloonTip(1500);
            }

            #region IDisposable Support

            private bool disposed = false;
            protected virtual void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    if (disposing)
                    {
                        icon.Icon = null;
                        icon.Visible = false;
                        icon.Dispose();

                        Click = null;
                        DoubleClick = null;
                        BalloonClick = null;
                    }

                    disposed = true;
                }
            }

            public void Dispose() => Dispose(true);
            #endregion
        }
    }
}