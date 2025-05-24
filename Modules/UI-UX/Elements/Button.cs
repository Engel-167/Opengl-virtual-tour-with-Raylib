using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Elements;

public class Button(Texture2D backgroundTexture, Texture2D hoverTexture, Vector2 position, int width, int height, int[] padding) : UiComponent(backgroundTexture, position, width, height)
{
    public string Text = string.Empty;
    public Font Font;
    public float FontSize;
    public float FontSpacing;
    public Color TextColor = Color.Black;
    public Color ClickTextColor = Color.White;
    public Color HoverTextColor = Color.Brown;
    public event EventHandler? Event;
    private bool _wasHovered;

    public override void Draw()
    {
        Position = new Vector2(Position.X, Position.Y); // This line is not needed, just for clarity
        HitBox = new Rectangle(Position.X, Position.Y, Width, Height);
        // Configure the 9-patch scaling
        NPatchInfo patchInfo = new NPatchInfo
        {
            Source = new Rectangle(0, 0, BackgroundTexture.Width, BackgroundTexture.Height),
            Left = padding[0], // Adjust these as required for your specific texture
            Top = padding[1],
            Right = padding[2],
            Bottom = padding[3],
            Layout = NPatchLayout.NinePatch
        };

        // Measure the text size
        Vector2 textSize = Raylib.MeasureTextEx(Font, Text, FontSize, FontSpacing);

        // Calculate text position based on button center and adjust for text size
        Vector2 textPosition = new(
            Position.X + Width / 2 - textSize.X / 2, // Center horizontally
            Position.Y + Height / 2 - textSize.Y / 2 // Center vertically
        );

        // Detect button states
        bool isHovered = Raylib.CheckCollisionPointRec(MouseCatcher.MousePosition, HitBox);
        bool isClicked = isHovered && Raylib.IsMouseButtonDown(MouseButton.Left);
        bool justClicked = isHovered && Raylib.IsMouseButtonReleased(MouseButton.Left);

        // Draw button texture
        if (isClicked)
        {
            Raylib.DrawTextureNPatch(hoverTexture, patchInfo, HitBox, Vector2.Zero, 0.0f, Color.Gray); // Optional: change texture for click
            Raylib.DrawTextPro(Font, Text, textPosition, Vector2.Zero, 0.0f, FontSize, FontSpacing, ClickTextColor);
            Raylib.PlaySound(Sfx.Click);
        }
        else if (isHovered)
        {
            Raylib.DrawTextureNPatch(hoverTexture, patchInfo, HitBox, Vector2.Zero, 0.0f, Color.White);
            Raylib.DrawTextPro(Font, Text, textPosition, Vector2.Zero, 0.0f, FontSize, FontSpacing, HoverTextColor);
            if (!_wasHovered)
            {
                Raylib.PlaySound(Sfx.Hover);    
            }
            _wasHovered = true;
        }
        else
        {
            _wasHovered = false;
            Raylib.DrawTextureNPatch(BackgroundTexture, patchInfo, HitBox, Vector2.Zero, 0.0f, Color.White);
            Raylib.DrawTextPro(Font, Text, textPosition, Vector2.Zero, 0.0f, FontSize, FontSpacing, TextColor);
        }

        // Trigger click event if mouse was released on the button
        if (justClicked)
        {
            TriggerEvent();
        }
    }
    
    

    private void TriggerEvent()
    {
        Event?.Invoke(this, EventArgs.Empty);
    }
}