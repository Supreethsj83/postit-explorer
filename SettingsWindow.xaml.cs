using System.Windows;
using PostItExplorer.Services;

namespace PostItExplorer.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            SecretsService.Load();
            GoogleClientIdBox.Text = SecretsService.GoogleClientId ?? "";
            MsClientIdBox.Text = SecretsService.MsClientId ?? "";
        }

        private async void GoogleLogin_Click(object sender, RoutedEventArgs e)
        {
            SecretsService.GoogleClientId = GoogleClientIdBox.Text;
            SecretsService.GoogleClientSecret = GoogleClientSecretBox.Password;
            SecretsService.Save();

            var ok = await GoogleDriveService.SignInAsync();
            MessageBox.Show(ok ? "Google signed in." : "Google sign-in failed.");
        }

        private void GoogleLogout_Click(object sender, RoutedEventArgs e)
        {
            GoogleDriveService.SignOut();
            MessageBox.Show("Signed out from Google.");
        }

        private async void MsLogin_Click(object sender, RoutedEventArgs e)
        {
            SecretsService.MsClientId = MsClientIdBox.Text;
            SecretsService.Save();
            var ok = await OneDriveService.SignInAsync();
            MessageBox.Show(ok ? "Microsoft signed in." : "Microsoft sign-in failed.");
        }

        private void MsLogout_Click(object sender, RoutedEventArgs e)
        {
            OneDriveService.SignOut();
            MessageBox.Show("Signed out from Microsoft.");
        }

        private void SyncNow_Click(object sender, RoutedEventArgs e)
        {
            SyncService.ManualSync();
        }
    }
}
