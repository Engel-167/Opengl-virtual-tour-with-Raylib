using System.Numerics;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX;

public class Button(Texture2D backgroundTexture, Texture2D hoverTexture, Vector2 position, int width, int height) : UiComponent(backgroundTexture, position, width, height)
{
    public event EventHandler? Event;
    public override void Draw()
    {
        // Configure the 9-patch scaling
        NPatchInfo patchInfo = new NPatchInfo
        {
            Source = new Rectangle(0, 0, BackgroundTexture.Width, BackgroundTexture.Height),
            Left = 40, // Adjust these as required for your specific texture
            Top = 40,
            Right = 40,
            Bottom = 40,
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
            Raylib.DrawTextPro(Font, Text, textPosition, Vector2.Zero, 0.0f, FontSize, FontSpacing, Color.White);
        }
        else if (isHovered)
        {
            Raylib.DrawTextureNPatch(hoverTexture, patchInfo, HitBox, Vector2.Zero, 0.0f, Color.White);
            Raylib.DrawTextPro(Font, Text, textPosition, Vector2.Zero, 0.0f, FontSize, FontSpacing, Color.Brown);
        }
        else
        {
            Raylib.DrawTextureNPatch(BackgroundTexture, patchInfo, HitBox, Vector2.Zero, 0.0f, Color.White);
            Raylib.DrawTextPro(Font, Text, textPosition, Vector2.Zero, 0.0f, FontSize, FontSpacing, Color.Black);
        }

        // Trigger click event if mouse was released on the button
        if (justClicked)
        {
            TriggerEvent();
        }
    }

    public void TriggerEvent()
    {
        Event?.Invoke(this, EventArgs.Empty);
    }
}