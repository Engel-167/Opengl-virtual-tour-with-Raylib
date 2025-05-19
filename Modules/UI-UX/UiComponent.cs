using System.Numerics;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX;

public abstract class UiComponent
{
    protected readonly Texture2D BackgroundTexture;
    protected Vector2 Position { get; set; }
    protected float Width { get; set; }
    protected float Height { get; set; }
    public Rectangle HitBox;
    public string Text = string.Empty;
    public Font Font;
    public float FontSize;
    public float FontSpacing;

    protected UiComponent(Texture2D backgroundTexture, Vector2 position, int width, int height)
    {
        BackgroundTexture = backgroundTexture;
        Position = position;
        Width = width;
        Height = height;
        
        HitBox = new Rectangle(Position.X, Position.Y, Width, Height);
    }

    public virtual void Draw()
    {
        
        NPatchInfo patchInfo = new NPatchInfo
        {
            Source = new Rectangle(0, 0, BackgroundTexture.Width, BackgroundTexture.Height),
            Left = 10, // Adjust these as required for your specific texture
            Top = 10,
            Right = 10,
            Bottom = 10,
            Layout = NPatchLayout.NinePatch
        };
        
        Raylib.DrawTextureNPatch(BackgroundTexture, patchInfo, HitBox, Vector2.Zero, 0.0f, Color.White);
    }
}