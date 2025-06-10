using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Elements;

public class Label : Button
{
    public Label(string text, Vector2 position, int width, int height): base(text, position, width, height)
    {
        Text = text;
        Position = position;
        Width = width;
        Height = height;
    }

    public override void Draw()
    {
        Position = new Vector2(Position.X, Position.Y); // This line is not needed, just for clarity
        HitBox = new Rectangle(Position.X, Position.Y, Width, Height);
        // Measure the text size
        Vector2 textSize = Raylib.MeasureTextEx(Font, Text, FontSize, FontSpacing);

        // Calculate text position based on button center and adjust for text size
        Vector2 textPosition = new(
            Position.X + Width / 2 - textSize.X / 2, // Center horizontally
            Position.Y + Height / 2 - textSize.Y / 2 // Center vertically
        );
        
        Raylib.DrawTextPro(Font, Text, textPosition, Vector2.Zero, 0.0f, FontSize, FontSpacing, TextColor);
    }
}