using System.Numerics;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX;

public static class MouseCatcher
{
    public static Vector2 MousePosition;

    public static MouseButton PressedButton = GetCurrentMouseButton();

    private static MouseButton GetCurrentMouseButton()
    {
        if (Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            return MouseButton.Left;
        }
        
        if (Raylib.IsMouseButtonReleased(MouseButton.Right))
        {
            return MouseButton.Right;
        }

        if (Raylib.IsMouseButtonReleased(MouseButton.Middle))
        {
            return MouseButton.Middle;    
        }

        return (MouseButton)(-1);

    }
    
    public static void UpdateMouseCatcher()
    {
        MousePosition = Raylib.GetMousePosition();
    }

    public static MouseButton Click(Rectangle hitBox)
    {
        if (Raylib.CheckCollisionPointRec(MousePosition, hitBox))
        {
            return GetCurrentMouseButton();
        }
        
        return (MouseButton)(-1);
    }
}