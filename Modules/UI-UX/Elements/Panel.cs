using System.Numerics;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Elements;

public class Panel(Texture2D backgroundTexture, Vector2 position, int width, int height, int[] padding) : UiComponent(backgroundTexture, position, width, height)
{
    public string Text = string.Empty;
    public Font Font;
    public float FontSize;
    public float FontSpacing;
    
    public override void Draw()
    {
        Position = new Vector2(Position.X, Position.Y); // This line is not needed, just for clarity
        HitBox = new Rectangle(Position.X, Position.Y, Width, Height);
        
        NPatchInfo patchInfo = new NPatchInfo
        {
            Source = new Rectangle(0, 0, BackgroundTexture.Width, BackgroundTexture.Height),
            Left = padding[0], // Adjust these as required for your specific texture
            Top = padding[1],
            Right = padding[2],
            Bottom = padding[3],
            Layout = NPatchLayout.NinePatch
        };
        
        Vector2 textSize = Raylib.MeasureTextEx(Font, Text, FontSize, FontSpacing);

        // Calculate text position based on button center and adjust for text size
        Vector2 textPosition = new(
            Position.X + Width / 2 - textSize.X / 2, // Center horizontally
            Position.Y + Height / 2 - textSize.Y / 2 // Center vertically
        );
        
        Raylib.DrawTextureNPatch(BackgroundTexture, patchInfo, HitBox, Vector2.Zero, 0.0f, Color.White);
        Raylib.DrawTextPro(Font, Text, textPosition, Vector2.Zero, 0.0f, FontSize, FontSpacing, Color.Black);
    }
}