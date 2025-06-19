using System.Diagnostics;
using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Elements;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Scenes;

public class CreditsScene(byte id, string windowTitle) : SceneObject(id, windowTitle)
{
    //Panels
    private Panel _backgroundPanel = null!;
    private Panel _flagPanel = null!;
    
    //buttons
    private Button _backButton = null!;
    
    private Button _engelButton = null!;
    private Button _isaacButton = null!;
    private Button _mathewButton = null!;
    private Button _samuelButton = null!;
    
    //Container
    private Container _container = null!;
    
    public override void InitScene()
    {
        Console.WriteLine("Credits Scene Initialized");
        _backgroundPanel = new Panel
        (
            Textures.BasicPanel, new Vector2(10, 10), 
            GetScreenWidth() - 20, GetScreenHeight() - 20,
            [40, 40, 40, 40]
        );

        _flagPanel = new Panel
        (
            Textures.Flag, new Vector2(GetScreenWidth() / 2f - 150, 20),
            300, 100,
            [55, 55, 55, 55]
        )
        {
            Text = Variables.Language.CreditsString,
            TextColor = Color.White,
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f
        };

        _backButton = new Button(
            Textures.BrownPanel,
            Textures.BrownPanelFocus,
            new Vector2((float)GetScreenWidth()/2 - 100, _backgroundPanel.Height - 110), 200, 80,
            [40, 40, 40, 40])
        {
            Text = Variables.Language.ExitString,
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        _backButton.Event += (_, _) =>
        {
            Core.Globals.Scenes.CurrentScene = Core.Globals.Scenes.Scene.Home;
        };
        
        _engelButton =  new Button
        (
            Textures.DisabledStateButton,
            Textures.DefaultStateButton,
            new Vector2(0, 0), 230, 80,
            [15,15,15,15]
        )
        {
            Text = "Engel-167",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        _engelButton.Event += (_, _) =>
        {
            Console.WriteLine("Github button");
            // Define the URL you want to open
            string url = "https://github.com/Engel-167"; // Replace with your desired URL
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
        
        _isaacButton =  new Button
        (
            Textures.DisabledStateButton,
            Textures.DefaultStateButton,
            new Vector2(0, 0), 230, 80,
            [15,15,15,15]
        )
        {
            Text = "josaak73",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        _isaacButton.Event += (_, _) =>
        {
            Console.WriteLine("Github button");
            // Define the URL you want to open
            string url = "https://github.com/josaak73"; // Replace with your desired URL
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
        
        _mathewButton =  new Button
        (
            Textures.DisabledStateButton,
            Textures.DefaultStateButton,
            new Vector2(0, 0), 230, 80,
            [15,15,15,15]
        )
        {
            Text = "MattewRojas",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        _mathewButton.Event += (_, _) =>
        {
            Console.WriteLine("Github button");
            // Define the URL you want to open
            string url = "https://github.com/MattewRojas"; // Replace with your desired URL
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
        
        _samuelButton =  new Button
        (
            Textures.DisabledStateButton,
            Textures.DefaultStateButton,
            new Vector2(0, 0), 230, 80,
            [15,15,15,15]
        )
        {
            Text = "SamuelSevilla",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
            TextColor = Color.White,
            HoverTextColor = Color.LightGray,
            ClickTextColor = Color.Black
        };
        _samuelButton.Event += (_, _) =>
        {
            Console.WriteLine("Github button");
            // Define the URL you want to open
            string url = "https://github.com/SamuelSevilla"; // Replace with your desired URL
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

        _container = new Container(ContainerOrientation.Vertical, 5, 5);
        _container.Components.Add(_engelButton);
        _container.Components.Add(_isaacButton);
        _container.Components.Add(_mathewButton);
        _container.Components.Add(_samuelButton);
        
        _container.Position = new Vector2(
            GetScreenWidth() / 2.0f - _container.GetWidth() / 2.0f, 
            _backgroundPanel.Height / 2.0f - _container.GetHeight() / 2.0f
        );
        
        Initialized = true;
    }

    public override void UpdateScene()
    {
        UpdateLayout();
        if (IsKeyPressed(KeyboardKey.Escape))
        {
            Core.Globals.Scenes.CurrentScene = Core.Globals.Scenes.Scene.Home;
        }
        
        BeginDrawing();
            DrawRectangle(0, 0, GetScreenWidth(), GetScreenHeight(), Color.SkyBlue);
            _backgroundPanel.Draw();
            _flagPanel.Draw();
            _backButton.Draw();
            
            _container.Draw();
            
            //DrawTextEx(Fonts.UbuntuM,Variables.Language.CreditsString, new Vector2(GetScreenWidth()/2f - 60, 50), 32f, 2f, Color.Black);
        EndDrawing();
    }

    public override void KillScene()
    {
        Console.WriteLine("Credits Scene Unloaded");
        Initialized = false;
    }

    private void UpdateLayout()
    {
        _backgroundPanel.Position = new Vector2(10,10);
        _backgroundPanel.Width = GetScreenWidth() - 20;
        _backgroundPanel.Height = GetScreenHeight() - 20;

        _flagPanel.Position = new Vector2(GetScreenWidth() / 2f - 150, 20);

        _backButton.Position = new Vector2((float)GetScreenWidth() / 2 - 100, _backgroundPanel.Height - 110);
        
        _container.Position = new Vector2(
            GetScreenWidth() / 2.0f - _container.GetWidth() / 2.0f, 
            _backgroundPanel.Height / 2.0f - _container.GetHeight() / 2.0f
        );
    }
}