using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Graph;

namespace PostItExplorer.Services
{
    public static class OneDriveService
    {
        private static IPublicClientApplication? _pca;
        private static GraphServiceClient? _graph;

        private static readonly string[] Scopes = new[] { "offline_access", "User.Read", "Files.ReadWrite.AppFolder" };

        public static async Task<bool> SignInAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SecretsService.MsClientId)) return false;

                _pca = PublicClientApplicationBuilder.Create(SecretsService.MsClientId)
                    .WithRedirectUri("http://localhost")
                    .WithDefaultRedirectUri()
                    .Build();

                var result = await _pca.AcquireTokenInteractive(Scopes)
                    .WithUseEmbeddedWebView(false)
                    .ExecuteAsync();

                var cache = (await _pca.UserTokenCache.SerializeMsalV3Async()).ToArray();
                SecretsService.SetMsCache(cache);

                _graph = new GraphServiceClient(new DelegateAuthenticationProvider(async req =>
                {
                    var accounts = await _pca.GetAccountsAsync();
                    var token = await _pca.AcquireTokenSilent(Scopes, accounts.FirstOrDefault()).ExecuteAsync();
                    req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
                }));
                return true;
            }
            catch
            {
                _graph = null;
                return false;
            }
        }

        private static async Task<GraphServiceClient?> EnsureAsync()
        {
            if (_graph != null) return _graph;
            if (string.IsNullOrWhiteSpace(SecretsService.MsClientId)) return null;
            _pca = PublicClientApplicationBuilder.Create(SecretsService.MsClientId)
                .WithRedirectUri("http://localhost")
                .WithDefaultRedirectUri()
                .Build();
            var cache = SecretsService.GetMsCache();
            if (cache != null)
            {
                await _pca.UserTokenCache.DeserializeMsalV3Async(new System.ReadOnlyMemory<byte>(cache), false);
            }

            _graph = new GraphServiceClient(new DelegateAuthenticationProvider(async req =>
            {
                var accounts = await _pca!.GetAccountsAsync();
                try
                {
                    var token = await _pca.AcquireTokenSilent(Scopes, accounts.FirstOrDefault()).ExecuteAsync();
                    req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
                }
                catch
                {
                    var token = await _pca.AcquireTokenInteractive(Scopes).WithUseEmbeddedWebView(false).ExecuteAsync();
                    req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
                }
            }));
            return _graph;
        }

        public static async Task UploadNotesAsync(string json)
        {
            var g = await EnsureAsync();
            if (g == null) return;
            var content = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await g.Me.Drive.Special["approot"].ItemWithPath("notes.json").Content.Request().PutAsync<Microsoft.Graph.DriveItem>(content);
        }

        public static async Task<string?> DownloadNotesAsync()
        {
            var g = await EnsureAsync();
            if (g == null) return null;
            try
            {
                var stream = await g.Me.Drive.Special["approot"].ItemWithPath("notes.json").Content.Request().GetAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch
            {
                return null;
            }
        }

        public static void SignOut()
        {
            _graph = null;
            SecretsService.SetMsCache(Array.Empty<byte>());
        }
    }
}
