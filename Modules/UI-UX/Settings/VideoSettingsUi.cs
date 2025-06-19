using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Settings;
using Opengl_virtual_tour_with_Raylib.Modules.Lighting;
using Opengl_virtual_tour_with_Raylib.Modules.Scenes;
using Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Elements;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Settings;

public class VideoSettingsUi
{
    private readonly Button? _fullscreenButton;

    private readonly Container? _container;

    public VideoSettingsUi()
    {
        _fullscreenButton = new Button(
            Textures.EnabledStateButton,
            Textures.DefaultStateButton,
            new Vector2(20 + 200, 50), 300, 80,
            [15, 15, 15, 15])
        {
            Text = Variables.Language.FullscreenString,
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        _fullscreenButton.Event += (_, _) =>
        {
            int monitor = GetCurrentMonitor();
            if (!Variables.AppSettings.Fullscreen)
            {
                _fullscreenButton.BackgroundTexture = Textures.EnabledStateButton;
                SetWindowSize(GetMonitorWidth(monitor), GetMonitorHeight(monitor));
                
                ToggleFullscreen();
                Variables.AppSettings.Fullscreen = true;
                Variables.SettingsMenu?.UpdateLayout();
                SettingsLoader.SaveSettings(Variables.SettingsFilePath, Variables.AppSettings);
            }
            else
            {
                _fullscreenButton.BackgroundTexture = Textures.DisabledStateButton;
                if (Variables.FirstTime)
                {
                    SetWindowSize(1280, 720);
                    Variables.AppSettings.ScreenWidth = 1280;
                    Variables.AppSettings.ScreenHeight = 720;
                    
                    Variables.FirstTime = false;
                }
                
                SetWindowSize(Variables.AppSettings.ScreenWidth, Variables.AppSettings.ScreenHeight);
                
                ToggleFullscreen();
                SetWindowPosition(GetMonitorWidth(monitor)/2 - GetScreenWidth()/2, GetMonitorHeight(monitor)/2 - GetScreenHeight()/2);
                
                Variables.AppSettings.Fullscreen = false;
                
                Variables.SettingsMenu?.UpdateLayout();
                
                SettingsLoader.SaveSettings(Variables.SettingsFilePath, Variables.AppSettings);
            }
        };
        
        Button shadowsButton = new Button(
            Variables.AppSettings.ShadowsEnabled?Textures.EnabledStateButton:Textures.DisabledStateButton,
            Textures.DefaultStateButton,
            new Vector2(20 + 200, 50), 200, 80,
            [15, 15, 15, 15])
        {
            Text = Variables.Language.ShadowsString,
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        shadowsButton.Event += (_, _) =>
        {
            Variables.AppSettings.ShadowsEnabled = !Variables.AppSettings.ShadowsEnabled;

            if (Variables.AppSettings.ShadowsEnabled)
            {
                shadowsButton.BackgroundTexture = Textures.EnabledStateButton;
                if (MainScene.WorldObjects != null) ShadowMap.Init(MainScene.WorldObjects);
            }
            else
            {
                shadowsButton.BackgroundTexture = Textures.DisabledStateButton;
                ShadowMap.UnloadShadowmapRenderTexture();
            }
    
            SettingsLoader.SaveSettings(Variables.SettingsFilePath, Variables.AppSettings);
        };
        
        _container = new Container(ContainerOrientation.Horizontal, 20, 10);
        _container.Components.Add(_fullscreenButton);
        _container.Components.Add(shadowsButton);
        
        _container.Position = new Vector2(
            GetScreenWidth() / 2.0f - _container.GetWidth() / 2.0f,
            GetScreenHeight() / 2.0f - _container.GetHeight() / 2.0f
        );
    }
    
    public void Draw()
    {
        if (_container != null)
        {
            _container.Components[0].Text = Variables.Language.FullscreenString;
            _container.Components[1].Text = Variables.Language.ShadowsString;


            _container?.Draw();
        }
    }
    
    public void UpdateLayout()
    {
        if (_container == null) return;

        _container.Position = new Vector2(
            GetScreenWidth() / 2.0f - _container.GetWidth() / 2.0f,
            GetScreenHeight() / 2.0f - _container.GetHeight() / 2.0f
        );
    }
}