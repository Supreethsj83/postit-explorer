using System;
using System.Threading;
using System.Threading.Tasks;

namespace PostItExplorer.Services
{
    public class SyncService
    {
        private System.Threading.Timer? _timer;
        public void Start()
        {
            SecretsService.Load();
            _timer = new System.Threading.Timer(async _ => await DoSync(), null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
        }
        public void Stop() => _timer?.Dispose();
        public void Kick() => _ = DoSync();

        public static void ManualSync() => _ = new SyncService().DoSync();

        private async Task DoSync()
        {
            try
            {
                var json = StorageService.GetRawJson();
                await GoogleDriveService.UploadNotesAsync(json);
                await OneDriveService.UploadNotesAsync(json);

                var g = await GoogleDriveService.DownloadNotesAsync();
                var m = await OneDriveService.DownloadNotesAsync();

                if (!string.IsNullOrEmpty(m))
                    StorageService.OverwriteRawJson(m!);
                else if (!string.IsNullOrEmpty(g))
                    StorageService.OverwriteRawJson(g!);
            }
            catch
            {
                // ignore sync errors silently
            }
        }
    }
}
