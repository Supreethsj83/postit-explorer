using System;
using System.Runtime.InteropServices;

namespace PostItExplorer.Services
{
    public static class HotkeyService
    {
        public const int WM_HOTKEY = 0x0312;
        public const int MOD_CONTROL = 0x0002;
        public const int HOTKEY_ID = 1;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public static void RegisterHotKey(IntPtr handle, int id, int modifiers, uint key)
        {
            UnregisterHotKey(handle, id);
            RegisterHotKey(handle, id, modifiers, key);
        }
    }
}
