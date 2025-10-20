using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PostItExplorer.Services
{
    public class SecretsModel
    {
        public string? GoogleClientId { get; set; }
        public string? GoogleClientSecret { get; set; }
        public byte[]? GoogleRefreshTokenEnc { get; set; }

        public string? MsClientId { get; set; }
        public byte[]? MsAccountEnc { get; set; } // serialized token cache
    }

    public static class SecretsService
    {
        private static readonly string Dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PostItExplorer");
        private static readonly string FilePath = Path.Combine(Dir, "secrets.json");

        private static SecretsModel _model = new();

        public static string? GoogleClientId { get => _model.GoogleClientId; set => _model.GoogleClientId = value; }
        public static string? GoogleClientSecret { get => _model.GoogleClientSecret; set => _model.GoogleClientSecret = value; }
        public static string? MsClientId { get => _model.MsClientId; set => _model.MsClientId = value; }

        public static void SetGoogleRefreshToken(string token)
        {
            _model.GoogleRefreshTokenEnc = ProtectedData.Protect(Encoding.UTF8.GetBytes(token), null, DataProtectionScope.CurrentUser);
            Save();
        }
        public static string? GetGoogleRefreshToken()
        {
            try
            {
                if (_model.GoogleRefreshTokenEnc == null) return null;
                var bytes = ProtectedData.Unprotect(_model.GoogleRefreshTokenEnc, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(bytes);
            }
            catch { return null; }
        }

        public static void SetMsCache(byte[] data)
        {
            _model.MsAccountEnc = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
            Save();
        }
        public static byte[]? GetMsCache()
        {
            try
            {
                if (_model.MsAccountEnc == null) return null;
                return ProtectedData.Unprotect(_model.MsAccountEnc, null, DataProtectionScope.CurrentUser);
            }
            catch { return null; }
        }

        public static void Save()
        {
            Directory.CreateDirectory(Dir);
            File.WriteAllText(FilePath, JsonSerializer.Serialize(_model, new JsonSerializerOptions { WriteIndented = true }));
        }

        public static void Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    _model = JsonSerializer.Deserialize<SecretsModel>(File.ReadAllText(FilePath)) ?? new();
                }
            }
            catch { _model = new(); }
        }
    }
}
