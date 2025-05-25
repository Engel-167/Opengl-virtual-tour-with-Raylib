using System.Numerics;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World.Hitboxes;

public class Hitbox
{ 
    public string Id { get; } 
    public Obb Box { get; }
    public Hitbox(string id, Vector3 center, Vector3 size, float angleYDegrees) 
    {
        Id = id;
        // Creates the rotation's quaternion over the Y-axis
        float angleRad = angleYDegrees * MathF.PI / 180.0f;
        var rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, angleRad);
        
        // The Obb needs the halfExtents
        Box = new Obb(center, size / 2f, rotation);
    }

    // Methods for collision, draw using the box
    public bool ContainsPoint(Vector3 point) => Box.ContainsPoint(point);
        
    public void DrawWireFrame(Color color) 
    { 
        Box.DrawWireFrame(color);
    }
}