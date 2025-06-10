using System.Numerics;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Elements;

public abstract class UiComponent
{
    public Texture2D BackgroundTexture;
    public Vector2 Position { get; set; }
    public float Width;
    public float Height;
    protected Rectangle HitBox;

    protected UiComponent(Texture2D? backgroundTexture, Vector2 position, int width, int height)
    {
        if (backgroundTexture != null) BackgroundTexture = (Texture2D)backgroundTexture;
        Position = position;
        Width = width;
        Height = height;
        
        HitBox = new Rectangle(Position.X, Position.Y, Width, Height);
    }

    protected UiComponent(Vector2 position, int width, int height)
    {
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