using System.Numerics;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX;

public static class MouseCatcher
{
    public static Vector2 MousePosition = Raylib.GetMousePosition();

    public static MouseButton PressedButton = GetCurrentMouseButton();

    private static MouseButton GetCurrentMouseButton()
    {
        if (Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            return MouseButton.Left;
        }
        else if (Raylib.IsMouseButtonReleased(MouseButton.Right))
        {
            return MouseButton.Right;
        }
        else if (Raylib.IsMouseButtonReleased(MouseButton.Middle))
        {
            return MouseButton.Middle;    
        }
        else
        {
            return (MouseButton)(-1);
        }
        
    }
    
    public static void UpdateMouseCatcher()
    {
        MousePosition = Raylib.GetMousePosition();
    }
}