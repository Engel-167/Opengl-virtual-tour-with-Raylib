using Tomlyn;

namespace Opengl_virtual_tour_with_Raylib.Modules.Core.Settings;

public static class SettingsLoader
{
    public static AppSettings LoadSettings(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Warning: Settings file not found at {filePath}. Using default settings.");
            return new AppSettings();
        }

        try
        {
            var toml = File.ReadAllText(filePath);
            return Toml.ToModel<AppSettings>(toml);
        }
        catch (TomlException ex)
        {
            Console.WriteLine($"Error parsing {filePath}: {ex.Message}");
            return new AppSettings();
        }
    }
    
    public static Language LoadLanguage(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Warning: Settings file not found at {filePath}. Using default settings.");
            return new Language();
        }

        try
        {
            var toml = File.ReadAllText(filePath);
            return Toml.ToModel<Language>(toml);
        }
        catch (TomlException ex)
        {
            Console.WriteLine($"Error parsing {filePath}: {ex.Message}");
            return new Language();
        }
    }

    public static void SaveSettings(string filePath, AppSettings settings)
    {
        try
        {
            var toml = Toml.FromModel(settings);
            File.WriteAllText(filePath, toml);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
        }
    }
}