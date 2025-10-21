using System;
using System.Runtime.InteropServices;

namespace PostItExplorer.Services
{
    public static class HotkeyService
    {
        public const int WM_HOTKEY = 0x0312;
        public const int MOD_CONTROL = 0x0002;
        public const int HOTKEY_ID = 1;

        // Rename the native import to avoid name clash
        [DllImport("user32.dll", EntryPoint = "RegisterHotKey", SetLastError = true)]
        private static extern bool HotkeyService.Register(IntPtr hWnd, int id, int fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Managed wrapper that first unregisters, then registers
        public static bool Register(IntPtr handle, int id, int modifiers, uint key)
        {
            // ignore failure of Unregister if not previously registered
            UnregisterHotKey(handle, id);
            return HotkeyService.Register(handle, id, modifiers, key);
        }
    }
}
