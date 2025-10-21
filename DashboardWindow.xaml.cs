using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PostItExplorer.Services;
using PostItExplorer.Models;

namespace PostItExplorer.Views
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            Grid.ItemsSource = StorageService.LoadAllNotes();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            var all = StorageService.LoadAllNotes();
            var q = (SearchBox.Text ?? "").ToLowerInvariant();
            var color = (ColorFilter.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Any";
            var filtered = all.Where(n =>
                (color == "Any" || n.Color == color) &&
                (string.IsNullOrEmpty(q) ||
                 (n.Path?.ToLowerInvariant().Contains(q) ?? false) ||
                 (n.Label?.ToLowerInvariant().Contains(q) ?? false) ||
                 (n.Body?.ToLowerInvariant().Contains(q) ?? false)))
                .ToList();
            Grid.ItemsSource = filtered;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Note n && !string.IsNullOrEmpty(n.Path))
            {
                var psi = new ProcessStartInfo("explorer.exe", $"/select,\"{n.Path}\"")
                {
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Note n && !string.IsNullOrEmpty(n.Path))
            {
                var win = new NoteWindow(n.Path);
                win.ShowDialog();
                LoadData();
            }
        }

        private void Sync_Click(object sender, RoutedEventArgs e)
        {
            SyncService.ManualSync();
        }
    }
}
