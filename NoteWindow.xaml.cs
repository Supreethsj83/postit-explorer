using System.Windows;
using PostItExplorer.Services;
using PostItExplorer.Models;

namespace PostItExplorer.Views
{
    public partial class NoteWindow : Window
    {
        private readonly string _path;
        public NoteWindow(string path)
        {
            InitializeComponent();
            _path = path;
            PathBox.Text = _path;

            var existing = StorageService.GetNoteForPath(_path);
            if (existing != null)
            {
                BodyBox.Text = existing.Body ?? "";
                LabelBox.Text = existing.Label ?? "";
                ColorBox.Text = existing.Color ?? "Yellow";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var note = new Note
            {
                Path = _path,
                Body = BodyBox.Text,
                Label = LabelBox.Text,
                Color = (ColorBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Yellow"
            };
            StorageService.UpsertNote(note);
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
