using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace PostItExplorer.Services
{
    public static class GoogleDriveService
    {
        private static DriveService? _svc;
        private static string[] Scopes = new[] { DriveService.Scope.DriveAppdata, "openid", "email", "profile" };

        public static async Task<bool> SignInAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SecretsService.GoogleClientId) || string.IsNullOrWhiteSpace(SecretsService.GoogleClientSecret))
                    return false;

                var secrets = new ClientSecrets
                {
                    ClientId = SecretsService.GoogleClientId,
                    ClientSecret = SecretsService.GoogleClientSecret
                };

                var cred = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    secrets, Scopes, "user", CancellationToken.None);

                if (cred.Token.RefreshToken != null)
                {
                    SecretsService.SetGoogleRefreshToken(cred.Token.RefreshToken);
                }

                _svc = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = cred,
                    ApplicationName = "PostIt Explorer"
                });
                return true;
            }
            catch
            {
                _svc = null;
                return false;
            }
        }

        public static void SignOut()
        {
            _svc = null;
            SecretsService.SetGoogleRefreshToken("");
        }

        private static async Task<DriveService?> EnsureAsync()
        {
            if (_svc != null) return _svc;
            if (string.IsNullOrWhiteSpace(SecretsService.GoogleClientId) || string.IsNullOrWhiteSpace(SecretsService.GoogleClientSecret))
                return null;
            try
            {
                var secrets = new ClientSecrets
                {
                    ClientId = SecretsService.GoogleClientId,
                    ClientSecret = SecretsService.GoogleClientSecret
                };
                var cred = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    secrets, Scopes, "user", CancellationToken.None);
                _svc = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = cred,
                    ApplicationName = "PostIt Explorer"
                });
                return _svc;
            }
            catch
            {
                return null;
            }
        }

        public static async Task UploadNotesAsync(string json)
        {
            var svc = await EnsureAsync();
            if (svc == null) return;

            var req = svc.Files.List();
            req.Spaces = "appDataFolder";
            req.Q = "name = 'notes.json'";
            var resp = await req.ExecuteAsync();
            var existing = resp.Files?.FirstOrDefault();

            using var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            if (existing == null)
            {
                var meta = new Google.Apis.Drive.v3.Data.File
                {
                    Name = "notes.json",
                    Parents = new[] { "appDataFolder" }
                };
                var create = svc.Files.Create(meta, ms, "application/json");
                create.Fields = "id";
                await create.UploadAsync();
            }
            else
            {
                var update = svc.Files.Update(new Google.Apis.Drive.v3.Data.File(), existing.Id, ms, "application/json");
                await update.UploadAsync();
            }
        }

        public static async Task<string?> DownloadNotesAsync()
        {
            var svc = await EnsureAsync();
            if (svc == null) return null;

            var req = svc.Files.List();
            req.Spaces = "appDataFolder";
            req.Q = "name = 'notes.json'";
            var resp = await req.ExecuteAsync();
            var existing = resp.Files?.FirstOrDefault();
            if (existing == null) return null;

            using var ms = new MemoryStream();
            await svc.Files.Get(existing.Id).DownloadAsync(ms);
            return System.Text.Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
