using System.Diagnostics;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX;

public class HomeUi
{
    private readonly Button? _startButton;
    private readonly Button? _settingsButton;
    private readonly Button? _creditsButton;
    private readonly Button? _exitButton;
    private readonly Button? _githubButton;

    public HomeUi(Action action)
    {
        Sound sound = LoadSound("Assets/UI-UX/Buttons/buttonfx.wav");
        //Start Button
        _startButton = new Button(
            Core.Globals.Textures.BasicPanel,
            Core.Globals.Textures.BasicPanelFocus,
            new Vector2(GetScreenWidth() / 2.0f - 125f, GetScreenHeight() / 2.0f - 50f), 250, 100,
            [40,40,40,40])
        {
            Text = "Iniciar",
            Font = Core.Globals.Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f,
        };
        _startButton.Event += (_, _) =>
        {
            PlaySound(sound);
            action.Invoke();
            Console.WriteLine("Start button");
        };

        //Settings Button
        _settingsButton = new Button(
            Core.Globals.Textures.BasicPanel,
            Core.Globals.Textures.BasicPanelFocus,
            new Vector2(GetScreenWidth() / 2.0f - 125f, GetScreenHeight() / 2.0f + 55f), 250, 100,
            [40,40,40,40])
        {
            Text = "Ajustes",
            Font = Core.Globals.Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f
        };
        _settingsButton.Event += (_, _) =>
        {
            PlaySound(sound);
            Console.WriteLine("Settings button");
        };

        //Credits button
        _creditsButton = new Button(
            Core.Globals.Textures.BasicPanel,
            Core.Globals.Textures.BasicPanelFocus,
            new Vector2(GetScreenWidth() / 2.0f - 125f, GetScreenHeight() / 2.0f + 160f), 250, 100,
            [40,40,40,40])
        {
            Text = "Creditos",
            Font = Core.Globals.Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f
        };
        _creditsButton.Event += (_, _) =>
        {
            PlaySound(sound);
            Console.WriteLine("Credits button");
        };

        //Exit button
        _exitButton = new Button(
            Core.Globals.Textures.BasicPanel,
            Core.Globals.Textures.BasicPanelFocus,
            new Vector2(GetScreenWidth() / 2.0f - 125f, GetScreenHeight() / 2.0f + 265f), 250, 100,
            [40,40,40,40])
        {
            Text = "Salir",
            Font = Core.Globals.Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f
        };
        _exitButton.Event += (_, _) =>
        {
            PlaySound(sound);
            Thread.Sleep(500);
            Console.WriteLine("Exit button");
            CloseWindow();
        };

        //Github Button
        _githubButton = new Button(
            Core.Globals.Textures.GithubIcon,
            Core.Globals.Textures.GithubIconHover,
            new Vector2(20,20),
            64,
            64,
            [0,0,0,0])
        {
            Text = "",
            Font = Core.Globals.Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f
        };
        _githubButton.Event += (_, _) =>
        {
            PlaySound(sound);
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
    }

    public void Draw()
    {
        if (_startButton != null) _startButton.Draw();
        if (_settingsButton != null) _settingsButton.Draw();
        if (_creditsButton != null) _creditsButton.Draw();
        if (_exitButton != null) _exitButton.Draw();
        if (_githubButton != null) _githubButton.Draw();
    }
}