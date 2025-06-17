using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Elements;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX;

public class DialogBox
{
    private readonly Panel _backgroundPanel;
    private readonly Button _closeButton;

    public DialogBox()
    {
        _backgroundPanel = new Panel(
            Textures.BasicPanel,
            new Vector2(20, Raylib.GetScreenHeight()/2f),  // Position
            Raylib.GetScreenWidth() - 40, Raylib.GetScreenHeight() /2 - 20, // Width and Height
            [40, 40, 40, 40] // Padding
        )
        {
            Text = Variables.Dialogs.Dialog1,
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f
        };
        
        _closeButton = new Button(
            Textures.BlueExitButton,
            Textures.BlueExitButtonFocus,
            new Vector2(_backgroundPanel.Width - 30, Raylib.GetScreenHeight()/2.0f - 10), 60, 60,
            [0, 0, 0, 0])
        {
            Text = string.Empty,
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f
        };
        _closeButton.Event += (_, _) =>
        {
            Variables.IsDialogBoxEnabled = false;
            Console.WriteLine("Close button DialogBox");
        };
    }
    
    public void Draw()
    {
        UpdateLayout();
        _backgroundPanel.Draw();
        _closeButton.Draw();
    }

    private void UpdateText()
    {
        _backgroundPanel.Text = Variables.Dialogs.Dialog1;
    }

    public void UpdateLayout()
    {
        UpdateText();
        _backgroundPanel.Position = new Vector2(20, Raylib.GetScreenHeight()/2f);
        _backgroundPanel.Width = Raylib.GetScreenWidth() - 40;
        _backgroundPanel.Height = Raylib.GetScreenHeight() / 2f - 20;
        
        _closeButton.Position = new Vector2(_backgroundPanel.Width - 30, Raylib.GetScreenHeight() / 2.0f - 10);
    }
}