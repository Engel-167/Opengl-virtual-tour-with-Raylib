using Opengl_virtual_tour_with_Raylib.Modules.Core.Settings;
using Opengl_virtual_tour_with_Raylib.Modules.UI_UX;

namespace Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;

public static class Variables
{
    //Settings menu Variables
    public static bool IsSettingsMenuEnabled = false;
    public static SettingsUi? SettingsMenu;
    
    //Music variables
    public static byte CurrentBgMusic = 1;
    
    //Settings Variables
    public static readonly string SettingsFilePath = "ConfigurationFiles/Settings.toml";
    public static AppSettings AppSettings = SettingsLoader.LoadSettings(SettingsFilePath);
    
    //Language Variables
    public static readonly string EnglishLangFilePath = "ConfigurationFiles/Language/EN.toml";
    public static readonly string SpanishLangFilePath = "ConfigurationFiles/Language/ES.toml";
    public static Language Language = SettingsLoader.LoadLanguage(AppSettings.Language == "EN"?EnglishLangFilePath:SpanishLangFilePath);
    
    //Tracking First initialization
    public static bool FirstTime = false;
    
    //
    public static bool UpdateText = false;
}