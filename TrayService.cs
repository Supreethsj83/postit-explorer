using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using PostItExplorer.Views;

namespace PostItExplorer.Services
{
    public static class TrayService
    {
        private static TaskbarIcon? _tray;

        public static void Initialize()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _tray = new TaskbarIcon
                {
                    ToolTipText = "PostIt Explorer"
                };
                var menu = new System.Windows.Controls.ContextMenu();
                var dash = new System.Windows.Controls.MenuItem { Header = "Open Dashboard" };
                dash.Click += (_, __) => new DashboardWindow().Show();
                var settings = new System.Windows.Controls.MenuItem { Header = "Settings" };
                settings.Click += (_, __) => new SettingsWindow().ShowDialog();
                var exit = new System.Windows.Controls.MenuItem { Header = "Exit" };
                exit.Click += (_, __) => Application.Current.Shutdown();
                menu.Items.Add(dash);
                menu.Items.Add(settings);
                menu.Items.Add(new System.Windows.Controls.Separator());
                menu.Items.Add(exit);
                _tray.ContextMenu = menu;
            });
        }
    }
}
