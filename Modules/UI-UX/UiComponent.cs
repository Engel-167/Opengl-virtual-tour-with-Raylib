using System.Numerics;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX;

public abstract class UiComponent
{
    private readonly Texture2D _backgroundTexture;
    private readonly Texture2D _hoverTexture;
    private Vector2 Position { get; set; }
    private int Width { get; set; }
    private int Height { get; set; }
    private Rectangle HitBox { get; set; }
    public string Text { get; set; } = string.Empty;

    protected UiComponent(Texture2D backgroundTexture, Vector2 position, int width, int height)
    {
        _backgroundTexture = backgroundTexture;
        _hoverTexture = backgroundTexture;
        Position = position;
        Width = width;
        Height = height;
        
        HitBox = new Rectangle(Position.X, Position.Y, Width, Height);
    }
    
    protected UiComponent(Texture2D backgroundTexture, Texture2D hoverTexture, Vector2 position, int width, int height)
    {
        _backgroundTexture = backgroundTexture;
        _hoverTexture = hoverTexture;
        Position = position;
        Width = width;
        Height = height;
        
        HitBox = new Rectangle(Position.X, Position.Y, Width, Height);
    }

    public void Draw()
    {
        /*if (Raylib.CheckCollisionPointRec(MouseCatcher.MousePosition, HitBox))
        {
            
        }*/
        
        Raylib.DrawTextureV(!Raylib.CheckCollisionPointRec(MouseCatcher.MousePosition, HitBox) ? _backgroundTexture : _hoverTexture, Position, Color.White);
    }
    
    public abstract void Event();
}