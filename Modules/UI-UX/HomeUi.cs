using System.Diagnostics;
using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Elements;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX;

public class HomeUi
{
    private readonly Button? _githubButton;
    
    private readonly Container? _container;

    public HomeUi()
    {
        //Start Button
        Button startButton = new Button(
            Textures.BasicPanel,
            Textures.BasicPanelFocus,
            new Vector2(GetScreenWidth() / 2.0f - 125f, GetScreenHeight() / 2.0f - 50f), 250, 100,
            [40,40,40,40])
        {
            Text = Variables.Language.StartString,
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
        };
        startButton.Event += (_, _) =>
        {
            Thread.Sleep(300);
            Core.Globals.Scenes.CurrentScene = Core.Globals.Scenes.Scene.Main;
            Console.WriteLine("Start button");
        };

        //Settings Button
        Button settingsButton = new Button(
            Textures.BasicPanel,
            Textures.BasicPanelFocus,
            new Vector2(GetScreenWidth() / 2.0f - 125f, GetScreenHeight() / 2.0f + 55f), 250, 100,
            [40,40,40,40])
        {
            Text = Variables.Language.SettingsString,
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f
        };
        settingsButton.Event += (_, _) =>
        {
            Variables.IsSettingsMenuEnabled = true;
            Console.WriteLine("Settings button");
        };

        //Credits button
        Button creditsButton = new Button(
            Textures.BasicPanel,
            Textures.BasicPanelFocus,
            new Vector2(GetScreenWidth() / 2.0f - 125f, GetScreenHeight() / 2.0f + 160f), 250, 100,
            [40,40,40,40])
        {
            Text = Variables.Language.CreditsString,
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f
        };
        creditsButton.Event += (_, _) =>
        {
            Console.WriteLine("Credits button");
            Thread.Sleep(500);
            Core.Globals.Scenes.CurrentScene = Core.Globals.Scenes.Scene.Credits;
        };

        //Exit button
        Button exitButton = new Button(
            Textures.BasicPanel,
            Textures.BasicPanelFocus,
            new Vector2(GetScreenWidth() / 2.0f - 125f, GetScreenHeight() / 2.0f + 265f), 250, 100,
            [40,40,40,40])
        {
            Text = Variables.Language.ExitString,
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f
        };
        exitButton.Event += (_, _) =>
        {
            Variables.Buildings.Unload3DModels();
            Variables.Roads.Unload3DModels();
            Variables.Props.Unload3DModels();
            
            Thread.Sleep(500);
            Console.WriteLine("Exit button");
            Environment.Exit(0);
        };

        //Github Button
        _githubButton = new Button(
            Textures.GithubIcon,
            Textures.GithubIconHover,
            new Vector2(20,20),
            64,
            64,
            [0,0,0,0])
        {
            Text = "",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f
        };
        _githubButton.Event += (_, _) =>
        {
            Console.WriteLine("Github button");
            // Define the URL you want to open
            string url = "https://github.com/Engel-167/Opengl-virtual-tour-with-Raylib"; // Replace with your desired URL
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // This is important for opening URLs
                });
            }
            catch (Exception ex)
            {
                // Handle any exceptions, e.g., if the browser cannot be opened
                Console.WriteLine($"Error opening URL: {ex.Message}");
            }
        };

        _container = new Container(ContainerOrientation.Vertical, 0, 5);
        _container.Components.Add(startButton);
        _container.Components.Add(settingsButton);
        _container.Components.Add(creditsButton);
        _container.Components.Add(exitButton);

        int lowerPoint = GetScreenHeight() / 2 - 50 + _container.GetHeight();
        if (GetScreenHeight() < 800 == false)
        {
            _container.Position = new Vector2(GetScreenWidth() / 2.0f - _container.GetWidth() / 2.0f, GetScreenHeight() / 2.0f - 50f);
        }
        else
        {
            // Adjust the position of the container if it goes out of bounds
            if (lowerPoint > GetScreenHeight() - 40)
            {
                int offset = lowerPoint - (GetScreenHeight() - 40);
                _container.Position = new Vector2(GetScreenWidth() / 2.0f - _container.GetWidth() / 2.0f, GetScreenHeight() / 2.0f - 50f - offset);
            }
            else
            {
                _container.Position = new Vector2(GetScreenWidth() / 2.0f - _container.GetWidth() / 2.0f, GetScreenHeight() / 2.0f - 50f);
            }
        }
        
        

    }

    public void Draw()
    {
        if (IsWindowResized())
        {
            UpdateLayout();
        }
        
        if (_githubButton != null) _githubButton.Draw();

        if (Variables.UpdateText)
        {
            if (_container != null)
            {
                _container.Components[0].Text = Variables.Language.StartString;
                _container.Components[1].Text = Variables.Language.SettingsString;
                _container.Components[2].Text = Variables.Language.CreditsString;
                _container.Components[3].Text = Variables.Language.ExitString;
            }
        }
        
        if (_container != null) _container.Draw();
    }

    public void UpdateLayout()
    {
        if (_container == null) return;

        int lowerPoint = GetScreenHeight() / 2 - 50 + _container.GetHeight();
        if (GetScreenHeight() >= 800)
        {
            _container.Position = new Vector2(GetScreenWidth() / 2f - _container.GetWidth() / 2f, GetScreenHeight() / 2f - 50f);
        }
        else
        {
            if (lowerPoint > GetScreenHeight() - 40)
            {
                int offset = lowerPoint - (GetScreenHeight() - 40);
                _container.Position = new Vector2(GetScreenWidth() / 2f - _container.GetWidth() / 2f, GetScreenHeight() / 2f - 50f - offset);
            }
            else
            {
                _container.Position = new Vector2(GetScreenWidth() / 2f - _container.GetWidth() / 2f, GetScreenHeight() / 2f - 50f);
            }
        }
    }
}