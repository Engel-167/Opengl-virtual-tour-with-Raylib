using System.Numerics;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX;

public class Button(Texture2D backgroundTexture, Texture2D hoverTexture, Vector2 position, int width, int height) : UiComponent(backgroundTexture, hoverTexture, position, width, height)
{
    public override void Event()
    {
        throw new NotImplementedException();
    }
}