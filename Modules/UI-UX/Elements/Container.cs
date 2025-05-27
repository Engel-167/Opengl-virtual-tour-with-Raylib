using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;

namespace Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Elements;

public class Container
{
    public readonly List<Button> Components = new();

    private readonly ContainerOrientation _orientation;
    
    public Vector2 Position;

    private readonly int _gaps;
    private readonly int _spacing;

    private int _height;
    private int _width;
    
    public int GetWidth()
    {
        _width = CalculateWidth();
        return _width;
    }
    
    public int GetHeight()
    {
        _height = CalculateHeight();
        return _height;
    }

    public Container (ContainerOrientation orientation,int gaps, int spacing)
    {
        _orientation = orientation;
        _gaps = gaps;
        _spacing = spacing;
    }
    
    public void Draw()
    {
        if (Components.Count == 0)
            return;

        _height = CalculateHeight();
        _width = CalculateWidth();

        int x = (int)Position.X + _gaps;
        int y = (int)Position.Y + _gaps;

        foreach (var obj in Components)
        {
            obj.Position = new Vector2(x, y);
            obj.Draw();

            if (_orientation == ContainerOrientation.Horizontal)
                x += (int)obj.Width + _spacing;
            else
                y += (int)obj.Height + _spacing;
        }
    }

    private int CalculateHeight()
    {
        if (_orientation == ContainerOrientation.Horizontal)
        {
            int maxHeight = 0;
            foreach (var obj in Components)
            {
                if (obj.Height > maxHeight)
                    maxHeight = (int)obj.Height;
            }
            return maxHeight + _gaps * 2;
        }

        // Vertical
        int totalHeight = 0;
        foreach (var obj in Components)
        {
            totalHeight += (int)obj.Height;
        }
        totalHeight += _spacing * (Components.Count - 1); // spacing between buttons
        totalHeight += _gaps * 2; // gaps at both ends
        return totalHeight;
    }
    
    private int CalculateWidth()
    {
        if (_orientation == ContainerOrientation.Horizontal)
        {
            int totalWidth = 0;
            foreach (var obj in Components)
            {
                totalWidth += (int)obj.Width;
            }
            totalWidth += _spacing * (Components.Count - 1); // spacing between buttons
            totalWidth += _gaps * 2; // gaps at both ends
            return totalWidth;
        }

        int maxWidth = 0;
        foreach (var obj in Components)
        {
            if (obj.Width > maxWidth)
                maxWidth = (int)obj.Width;
        }
        return maxWidth + _gaps * 2;
    }
}