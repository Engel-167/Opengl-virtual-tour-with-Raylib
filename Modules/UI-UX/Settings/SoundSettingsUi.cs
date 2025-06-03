using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules.Core;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Settings;
using Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Elements;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Settings;

public class SoundSettingsUi
{
    private readonly Container? _container;

    public SoundSettingsUi()
    {
        bool musicIsPaused = !Variables.AppSettings.MusicEnabled;
        
        Button lowerVolumeButton = new Button
        (
            Textures.EnabledStateButton,
            Textures.DisabledStateButton,
            new Vector2(0, 0), 80, 80,
            [15,15,15,15]
        )
        {
            Text = "-",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        lowerVolumeButton.Event += (_, _) =>
        {
            Variables.AppSettings.MasterVolume -= 0.05f;
            
            AudioManager.CheckVolumeMaxTreshold();
            
            Raylib.SetMasterVolume(Variables.AppSettings.MasterVolume);
            SettingsLoader.SaveSettings(Variables.SettingsFilePath, Variables.AppSettings);
        };
        
        Button pauseVolumeButton = new Button
        (
            Textures.DisabledStateButton,
            Textures.DefaultStateButton,
            new Vector2(0, 0), 80, 80,
            [15,15,15,15]
        )
        {
            Text = Variables.AppSettings.MusicEnabled?">>": "::",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        pauseVolumeButton.Event += (_, _) =>
        {
            musicIsPaused  = !musicIsPaused;

            if (musicIsPaused)
            {
                Variables.AppSettings.MusicEnabled = false;
                SettingsLoader.SaveSettings(Variables.SettingsFilePath, Variables.AppSettings);
                pauseVolumeButton.Text = "::";
            }
            else
            {
                Variables.AppSettings.MusicEnabled = true;
                SettingsLoader.SaveSettings(Variables.SettingsFilePath, Variables.AppSettings);
                pauseVolumeButton.Text = ">>";
            }
            
        };
        
        Button uperVolumeButton = new Button
        (
            Textures.EnabledStateButton,
            Textures.DisabledStateButton,
            new Vector2(0, 0), 80, 80,
            [15,15,15,15]
        )
        {
            Text = " + ",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        uperVolumeButton.Event += (_, _) =>
        {
            Variables.AppSettings.MasterVolume += 0.05f;
            
            AudioManager.CheckVolumeMaxTreshold();
            
            Raylib.SetMasterVolume(Variables.AppSettings.MasterVolume);
            SettingsLoader.SaveSettings(Variables.SettingsFilePath, Variables.AppSettings);
        };

        _container = new Container(ContainerOrientation.Horizontal, 5, 5);
        _container.Components.Add(lowerVolumeButton);
        _container.Components.Add(pauseVolumeButton);
        _container.Components.Add(uperVolumeButton);
        
        _container.Position = new Vector2(
            Raylib.GetScreenWidth() / 2.0f - _container.GetWidth() / 2.0f,
            Raylib.GetScreenHeight() / 2.0f - _container.GetHeight() / 2.0f
        );
    }
    
    public void Draw()
    {
        UpdateLayout();
        _container?.Draw();
    }
    
    public void UpdateLayout()
    {
        if (_container == null) return;

        _container.Position = new Vector2(
            Raylib.GetScreenWidth() / 2.0f - _container.GetWidth() / 2.0f,
            Raylib.GetScreenHeight() / 2.0f - _container.GetHeight() / 2.0f
        );
    }
}