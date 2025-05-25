using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Elements;
using Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Settings;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX;

public class SettingsUi
{
    private readonly Panel? _backgroundPanel;
    private readonly Button? _closeButton;
    
    private readonly Button? _homeButton;

    private readonly Container? _container;
    
    private readonly VideoSettingsUi? _videoSettingsUi;
    
    private enum CurrentSettings
    {
        General,
        Video,
        Sound,
        Controls
    }
    
    private CurrentSettings _currentSettings = CurrentSettings.General;

    public SettingsUi()
    {
        _backgroundPanel = new Panel(
            Textures.BasicPanel,
            new Vector2(20, 20),  // Position
            Raylib.GetScreenWidth() - 40, Raylib.GetScreenHeight() - 40, // Width and Height
            [40, 40, 40, 40] // Padding
        )
        {
            Text = "Ajustes",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f
        };
        
        _closeButton = new Button(
            Textures.BlueExitButton,
            Textures.BlueExitButtonFocus,
            new Vector2(_backgroundPanel.Width - 30, 10), 60, 60,
            [0, 0, 0, 0])
        {
            Text = "",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f
        };
        _closeButton.Event += (_, _) =>
        {
            Variables.IsSettingsMenuEnabled = false;
            Console.WriteLine("Close button");
        };
        
        Button generalSettings = new Button(
            Textures.BrownPanel,
            Textures.BrownPanelFocus,
            new Vector2(20 + 200, 50), 200, 80,
            [40, 40, 40, 40])
        {
            Text = "General",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        generalSettings.Event += (_, _) =>
        {
            _currentSettings = CurrentSettings.General;
            _backgroundPanel.Text = "General Settings";
            Console.WriteLine("General Settings button");
        };

        Button videoSettings = new Button(
            Textures.BrownPanel,
            Textures.BrownPanelFocus,
            new Vector2(440,50), 200, 80,
            [40, 40, 40, 40])
        {
            Text = "Video",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        videoSettings.Event += (_, _) =>
        {
            _currentSettings = CurrentSettings.Video;
            _backgroundPanel.Text = "Video Settings";
            Console.WriteLine("Video Settings button");
        };
        
        Button soundSettings = new Button(
            Textures.BrownPanel,
            Textures.BrownPanelFocus,
            new Vector2(440 + 220, 50), 200, 80,
            [40, 40, 40, 40])
        {
            Text = "Sonido",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        soundSettings.Event += (_, _) =>
        {
            _currentSettings = CurrentSettings.Sound;
            _backgroundPanel.Text = "Sound Settings";
            Console.WriteLine("Sound Settings button");
        };
        
        Button controlsSettings = new Button(
            Textures.BrownPanel,
            Textures.BrownPanelFocus,
            new Vector2(660 + 220, 50), 200, 80,
            [40, 40, 40, 40])
        {
            Text = "Controles",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        controlsSettings.Event += (_, _) =>
        {
            _currentSettings = CurrentSettings.Controls;
            _backgroundPanel.Text = "Controls Settings";
            Console.WriteLine("Controls Settings button");
        };
        
        
        //Home button
        _homeButton = new Button(
            Textures.BrownPanel,
            Textures.BrownPanelFocus,
            new Vector2((float)Raylib.GetScreenWidth()/2 - 100, _backgroundPanel.Height - 110), 200, 80,
            [40, 40, 40, 40])
        {
            Text = "Salir",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        _homeButton.Event += (_, _) =>
        {
            Console.WriteLine("Home button");
            Thread.Sleep(500);
            Core.Globals.Scenes.CurrentScene = Core.Globals.Scenes.Scene.Home;
            Variables.IsSettingsMenuEnabled = false;
        };
        
        
        // Add components to the Container
        _container = new Container(ContainerOrientation.Horizontal, 50, 20);
        _container.Components.Add(generalSettings);
        _container.Components.Add(videoSettings);
        _container.Components.Add(soundSettings);
        _container.Components.Add(controlsSettings);
        
        _container.Position = new Vector2(Raylib.GetScreenWidth() / 2.0f - _container.GetWidth() / 2.0f, 10);
        
        _videoSettingsUi = new VideoSettingsUi();
    }
    
    public void Draw()
    {
        if (Raylib.IsWindowResized())
        {
            UpdateLayout();
        }
        
        _backgroundPanel?.Draw();
        _closeButton?.Draw();
        _container?.Draw();

        switch (_currentSettings)
        {
            case CurrentSettings.General:
            {
                // Draw general settings UI here
                
            }
                break;
            case CurrentSettings.Video:
            {
                _videoSettingsUi?.Draw();
            }
                break;
            case CurrentSettings.Sound:
            {
                // Draw sound settings UI here
                
            }
                break;
            case CurrentSettings.Controls:
            {
                // Draw controls settings UI here
                
            }
                break;
        }
        
        if (_homeButton != null && Core.Globals.Scenes.CurrentScene == Core.Globals.Scenes.Scene.Main) _homeButton.Draw();
    }

    public void UpdateLayout()
    {
        // Update background panel size and position
        if (_backgroundPanel != null)
        {
            _backgroundPanel.Position = new Vector2(20, 20); // Ensure position is reset
            _backgroundPanel.Width = Raylib.GetScreenWidth() - 40;
            _backgroundPanel.Height = Raylib.GetScreenHeight() - 40;

            // Update close button position
            if (_closeButton != null) _closeButton.Position = new Vector2(_backgroundPanel.Width - 30, 10);

            // Update home button position
            if (_homeButton != null)
                _homeButton.Position = new Vector2(Raylib.GetScreenWidth() / 2f - 100, _backgroundPanel.Height - 110);
        }

        // Update container position (centered horizontally, y=10)
        if (_container != null)
            _container.Position = new Vector2(Raylib.GetScreenWidth() / 2f - _container.GetWidth() / 2f, 10);
        
        _videoSettingsUi?.UpdateLayout();
    }
}