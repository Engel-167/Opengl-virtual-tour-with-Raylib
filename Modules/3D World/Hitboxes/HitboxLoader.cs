using System.Numerics;
using Raylib_cs;
using Tomlyn;
using Tomlyn.Model;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World.Hitboxes;

public class HitboxLoader(string path)
{
    public readonly List<Hitbox> Cajas = LoadFromToml(path);

    public static List<Hitbox> LoadFromToml(string path)
    {
        var hitboxes = new List<Hitbox>();

        var doc = Toml.Parse(File.ReadAllText(path));
        var model = doc.ToModel();

        if (!model.TryGetValue("hitboxes", out var value))
            return hitboxes;

        if (value is not TomlTableArray array) return hitboxes;
        foreach (var entry in array)
        {
            var id = entry["id"] as string;
            var centerArray = entry["center"] as TomlArray;
            var sizeArray = entry["size"] as TomlArray;
            var angle = 0f;
            if (entry.TryGetValue("angle", out var value1))
            {
                // Puede ser int o float en TOML, asegúrate de convertir correctamente
                angle = Convert.ToSingle(value1);
            }

            var center = new Vector3(
                float.Parse(centerArray?[0]?.ToString() ?? string.Empty),
                float.Parse(centerArray?[1]?.ToString() ?? string.Empty),
                float.Parse(centerArray?[2]?.ToString() ?? string.Empty)
            );

            var size = new Vector3(
                float.Parse(sizeArray?[0]?.ToString() ?? string.Empty),
                float.Parse(sizeArray?[1]?.ToString() ?? string.Empty),
                float.Parse(sizeArray?[2]?.ToString() ?? string.Empty)
            );

            if (id != null)
                hitboxes.Add(new Hitbox(id, center, size, angle));
        }

        return hitboxes;
    }

    public void DrawBoundingBoxes(Color color)
    {
        foreach (Hitbox caja in Cajas)
        {
            // Dibujar la OBB rotada para depuración
            caja.DrawWireFrame(color);
        }
    }
}