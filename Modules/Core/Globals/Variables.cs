using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Buildings;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Props;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Roads;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Settings;
using Opengl_virtual_tour_with_Raylib.Modules.UI_UX;

namespace Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;

public static class Variables
{
    //3d objects
    public static Buildings? Buildings;
    public static Roads? Roads;
    public static Props? Props;
    
    //Ui Variables
    public static bool IsSettingsMenuEnabled = false;
    public static bool IsDialogBoxEnabled = false;
    public static HomeUi? HomeUi;
    public static SettingsUi? SettingsMenu;
    public static DialogBox? DialogBox;
    
    //Settings Variables
    public static readonly string SettingsFilePath = "ConfigurationFiles/Settings.toml";
    public static AppSettings AppSettings = SettingsLoader.LoadSettings(SettingsFilePath);
    
    //Language Variables
    public static readonly string EnglishLangFilePath = "ConfigurationFiles/Language/EN.toml";
    public static readonly string SpanishLangFilePath = "ConfigurationFiles/Language/ES.toml";
    public static Language Language = SettingsLoader.LoadLanguage(AppSettings.Language == "EN"?EnglishLangFilePath:SpanishLangFilePath);
    //Dialogs Variables
    public static readonly string EnglishDialogFilePath = "ConfigurationFiles/Dialogs/EN-Dialogs.toml";
    public static readonly string SpanishDialogFilePath = "ConfigurationFiles/Dialogs/ES-Dialogs.toml";
    public static Dialogs.Dialogs DialogsStorage = SettingsLoader.LoadDialog(AppSettings.Language == "EN"?EnglishDialogFilePath:SpanishDialogFilePath);
    
    //Tracking First initialization
    public static bool FirstTime = false;
    
    //Tracking if the text of the UI should be updated
    public static bool UpdateText = false;
    
    //Interaction Variables
    public static bool CanInteract = false;
    public static string InteractableObjectId = string.Empty;
}