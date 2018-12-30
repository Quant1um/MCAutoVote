using MCAutoVote.Bootstrap;
using System.Windows.Forms;

namespace MCAutoVote.CLI
{
    [LoadModule]
    public static class CLITray
    {
        static CLITray()
        {
            Icon = CreateIcon();
        }

        public static NotifyIcon Icon { get; }

        private static NotifyIcon CreateIcon()
        {
            NotifyIcon icon = new NotifyIcon()
            {
                Text = Info.Name,
                BalloonTipTitle = Info.Name,
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(Info.ExecutablePath),
                Visible = true
            };

            icon.MouseDoubleClick += (sender, e) => CLIWindow.Hidden = !CLIWindow.Hidden;
            return icon;
        }

        public static void Bubble(string message, ToolTipIcon icon = ToolTipIcon.None)
        {
            Icon.BalloonTipIcon = icon;
            Icon.BalloonTipText = message;
            Icon.ShowBalloonTip(1500);
        }
    }
}
