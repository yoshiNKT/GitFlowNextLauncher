using System.IO;
using System.Text.Json;

namespace GitFlowNextLauncher.Model;

public static class AppSettings
{
    public static string? LastRepositoryPath { get; set; }

    public static string? InitialDirPath { get; set; }


    private static readonly string SettingsPath =
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "GitFlowNextLauncher",
            "settings.json");

    public static void Load()
    {
        if (!File.Exists(SettingsPath))
            return;

        try
        {
            var json = File.ReadAllText(SettingsPath);

            var settings = JsonSerializer.Deserialize<AppSettingsData>(json);

            LastRepositoryPath = settings?.LastRepositoryPath;
            InitialDirPath = settings?.InitialDirPath;
        }
        catch
        {
            LastRepositoryPath = null;
        }
    }

    public static void Save()
    {
        var directory = Path.GetDirectoryName(SettingsPath);

        if (directory is not null)
        {
            Directory.CreateDirectory(directory);
        }

        var settings = new AppSettingsData
        {
            LastRepositoryPath = LastRepositoryPath,
            InitialDirPath = InitialDirPath
        };

        var json = JsonSerializer.Serialize(
            settings,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(SettingsPath, json);
    }

    private sealed class AppSettingsData
    {
        public string? LastRepositoryPath { get; set; }
        public string? InitialDirPath { get; set; }
    }
}