using System;
using System.Windows;
using System.Windows.Interop;
using PostItExplorer.Services;

namespace PostItExplorer.Views
{
    public partial class HiddenBootstrapper : Window
    {
        private IntPtr _windowHandle;
        private HwndSource? _source;
        private SyncService? _sync;

        public HiddenBootstrapper()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _windowHandle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(WndProc);
            HotkeyService.RegisterHotKey(_windowHandle, HotkeyService.HOTKEY_ID, HotkeyService.MOD_CONTROL, (uint)System.Windows.Input.KeyInterop.VirtualKeyFromKey(System.Windows.Input.Key.Space));
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == HotkeyService.WM_HOTKEY && wParam.ToInt32() == HotkeyService.HOTKEY_ID)
            {
                ExplorerService.TryGetSelectedPath(out var path);
                if (!string.IsNullOrEmpty(path))
                {
                    var noteWin = new Views.NoteWindow(path!);
                    noteWin.ShowDialog();
                    _sync?.Kick();
                }
                handled = true;
            }
            return IntPtr.Zero;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TrayService.Initialize();
            _sync = new SyncService();
            _sync.Start();
            this.Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            HotkeyService.UnregisterHotKey(_windowHandle, HotkeyService.HOTKEY_ID);
            _source?.Dispose();
            _sync?.Stop();
        }
    }
}
