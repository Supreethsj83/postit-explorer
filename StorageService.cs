using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PostItExplorer.Models;

namespace PostItExplorer.Services
{
    public static class StorageService
    {
        private static readonly string AppDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PostItExplorer");
        private static readonly string DataPath = Path.Combine(AppDir, "data.json");

        private class Envelope { public List<Note> Notes { get; set; } = new(); }

        private static Envelope LoadEnvelope()
        {
            Directory.CreateDirectory(AppDir);
            if (!File.Exists(DataPath))
            {
                var empty = new Envelope();
                File.WriteAllText(DataPath, JsonSerializer.Serialize(empty, new JsonSerializerOptions { WriteIndented = true }));
                return empty;
            }
            try
            {
                var json = File.ReadAllText(DataPath);
                return JsonSerializer.Deserialize<Envelope>(json) ?? new Envelope();
            }
            catch
            {
                return new Envelope();
            }
        }

        private static void SaveEnvelope(Envelope env)
        {
            Directory.CreateDirectory(AppDir);
            var json = JsonSerializer.Serialize(env, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(DataPath, json);
        }

        public static List<Note> LoadAllNotes() => LoadEnvelope().Notes;

        public static Note? GetNoteForPath(string path)
        {
            var env = LoadEnvelope();
            return env.Notes.FirstOrDefault(n => string.Equals(n.Path, path, StringComparison.OrdinalIgnoreCase));
        }

        public static void UpsertNote(Note note)
        {
            var env = LoadEnvelope();
            var existing = env.Notes.FirstOrDefault(n => string.Equals(n.Path, note.Path, StringComparison.OrdinalIgnoreCase));
            if (existing == null)
            {
                env.Notes.Add(note);
            }
            else
            {
                existing.Body = note.Body;
                existing.Label = note.Label;
                existing.Color = note.Color;
            }
            SaveEnvelope(env);
        }

        public static string GetRawJson() => JsonSerializer.Serialize(new Envelope { Notes = LoadAllNotes() }, new JsonSerializerOptions { WriteIndented = true });
        public static void OverwriteRawJson(string json)
        {
            Directory.CreateDirectory(AppDir);
            File.WriteAllText(DataPath, json);
        }
    }
}
