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
    
    //Tracking First initialization
    public static bool FirstTime = false;
}