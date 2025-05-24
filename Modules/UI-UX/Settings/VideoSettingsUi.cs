using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
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
            Textures.BrownPanel,
            Textures.BrownPanelFocus,
            new Vector2(20 + 200, 50), 300, 80,
            [40, 40, 40, 40])
        {
            Text = "Pantalla Completa",
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
            if (!IsWindowFullscreen())
            {
                SetWindowSize(GetMonitorWidth(monitor), GetMonitorHeight(monitor));
                Variables.SettingsMenu?.UpdateLayout();
                ToggleFullscreen();
            }
            else
            {
                SetWindowSize(1280, 720);
                ToggleFullscreen();
                SetWindowPosition(GetMonitorWidth(monitor)/2 - 1280/2, GetMonitorHeight(monitor)/2 - 720/2);
                Variables.SettingsMenu?.UpdateLayout();
            }
        };
        
        Button shadowsButton = new Button(
            Textures.BrownPanel,
            Textures.BrownPanelFocus,
            new Vector2(20 + 200, 50), 200, 80,
            [40, 40, 40, 40])
        {
            Text = "Sombras",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        shadowsButton.Event += (_, _) =>
        {
            ShadowMap.Enabled = !ShadowMap.Enabled;

            if (ShadowMap.Enabled)
            {
                if (MainScene.WorldObjects != null) ShadowMap.Init(MainScene.WorldObjects);
            }
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
        _container?.Draw();
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