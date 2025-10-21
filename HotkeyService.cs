using System;
using System.Runtime.InteropServices;

namespace PostItExplorer.Services
{
    public static class HotkeyService
    {
        public const int WM_HOTKEY = 0x0312;
        public const int MOD_CONTROL = 0x0002;
        public const int HOTKEY_ID = 1;

        // Native P/Invoke (renamed to avoid clash)
        [DllImport("user32.dll", EntryPoint = "RegisterHotKey", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Managed wrapper
        public static bool Register(IntPtr handle, int id, int modifiers, uint key)
        {
            // Ignore failure if it wasn't registered before
            UnregisterHotKey(handle, id);
            return RegisterHotKey(handle, id, modifiers, key);
        }
    }
}
