using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Elements;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Settings;

public class ControlsSettingsUi
{
    private readonly Container _labelsContainer;
    private readonly Container _controlsContainer;
    
    public ControlsSettingsUi()
    {
        // Buttons
        Button forwardButton = new Button(
            Textures.EnabledStateButton,
            Textures.EnabledStateButton,
            new Vector2(250, 50), 50, 50,
            [10, 10, 10, 10])
        {
            Text = "W",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2.0f,
            TextColor = Color.White,
            HoverTextColor = Color.Gray,
            ClickTextColor = Color.DarkGray
        };
        
        Button backwardButton = new Button(
            Textures.EnabledStateButton,
            Textures.EnabledStateButton,
            new Vector2(250, 50), 50, 50,
            [10, 10, 10, 10])
        {
            Text = "S",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2.0f,
            TextColor = Color.White,
            HoverTextColor = Color.Gray,
            ClickTextColor = Color.DarkGray
        };
        
        Button leftButton = new Button(
            Textures.EnabledStateButton,
            Textures.EnabledStateButton,
            new Vector2(250, 50), 50, 50,
            [10, 10, 10, 10])
        {
            Text = "A",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2.0f,
            TextColor = Color.White,
            HoverTextColor = Color.Gray,
            ClickTextColor = Color.DarkGray
        };
        
        Button rightButton = new Button(
            Textures.EnabledStateButton,
            Textures.EnabledStateButton,
            new Vector2(250, 50), 50, 50,
            [10, 10, 10, 10])
        {
            Text = "D",
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2.0f,
            TextColor = Color.White,
            HoverTextColor = Color.Gray,
            ClickTextColor = Color.DarkGray
        };
        
        // Labels
        Label forwardLabel = new Label("Forward", new Vector2(50, 50), 200, 50)
        {
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2.0f,
            TextColor = Color.Black
        };
        
        Label backwardLabel = new Label("Backward", new Vector2(50, 100), 200, 50)
        {
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2.0f,
            TextColor = Color.Black
        };
        
        Label leftLabel = new Label("Left", new Vector2(50, 150), 200, 50)
        {
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2.0f,
            TextColor = Color.Black
        };
        
        Label rightLabel = new Label("Right", new Vector2(50, 200), 200, 50)
        {
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2.0f,
            TextColor = Color.Black
        };
        
        //Containers
        _controlsContainer = new Container(ContainerOrientation.Vertical, 4, 4);
        _controlsContainer.Components.Add(forwardButton);
        _controlsContainer.Components.Add(backwardButton);
        _controlsContainer.Components.Add(leftButton);
        _controlsContainer.Components.Add(rightButton);
        
        _labelsContainer = new Container(ContainerOrientation.Vertical, 4, 4);
        _labelsContainer.Components.Add(forwardLabel);
        _labelsContainer.Components.Add(backwardLabel);
        _labelsContainer.Components.Add(leftLabel);
        _labelsContainer.Components.Add(rightLabel);

        // Position the containers in the center of the screen
        _controlsContainer.Position = new Vector2(
            Raylib.GetScreenWidth() / 2.0f - _controlsContainer.GetWidth() / 2.0f - 60,
            Raylib.GetScreenHeight() / 2.0f - _controlsContainer.GetHeight() / 2.0f
        );
        
        _labelsContainer.Position = new Vector2(
            Raylib.GetScreenWidth() / 2.0f - _labelsContainer.GetWidth() / 2.0f + 60,
            Raylib.GetScreenHeight() / 2.0f - _labelsContainer.GetHeight() / 2.0f
        );
    }
    
    public void UpdateLayout()
    {
        _controlsContainer.Position = new Vector2(
            Raylib.GetScreenWidth() / 2.0f - _controlsContainer.GetWidth() / 2.0f - 60,
            Raylib.GetScreenHeight() / 2.0f - _controlsContainer.GetHeight() / 2.0f
        );
        
        _labelsContainer.Position = new Vector2(
            Raylib.GetScreenWidth() / 2.0f - _labelsContainer.GetWidth() / 2.0f + 60,
            Raylib.GetScreenHeight() / 2.0f - _labelsContainer.GetHeight() / 2.0f
        );
    }
    
    public void Draw()
    {
        UpdateLayout();
        _labelsContainer.Draw();
        _controlsContainer.Draw();
    }
}