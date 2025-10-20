using System;

namespace PostItExplorer.Services
{
    public static class ExplorerService
    {
        public static bool TryGetSelectedPath(out string? path)
        {
            path = null;
            try
            {
                Type shellAppType = Type.GetTypeFromProgID("Shell.Application")!;
                dynamic shell = Activator.CreateInstance(shellAppType)!;
                var windows = shell.Windows();
                foreach (var w in windows)
                {
                    try
                    {
                        dynamic ie = w;
                        string fullName = (string)(ie.FullName ?? "");
                        if (!fullName.EndsWith("explorer.exe", StringComparison.OrdinalIgnoreCase)) continue;
                        dynamic doc = ie.Document;
                        dynamic selected = doc?.SelectedItems();
                        if (selected != null && selected.Count > 0)
                        {
                            dynamic item = selected.Item(0);
                            string candidate = item.Path;
                            if (!string.IsNullOrEmpty(candidate))
                            {
                                path = candidate;
                                return true;
                            }
                        }
                    }
                    catch { /* ignore non-explorer windows */ }
                }
            }
            catch { }
            return false;
        }
    }
}
