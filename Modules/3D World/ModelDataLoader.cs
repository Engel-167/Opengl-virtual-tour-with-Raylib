using Tomlyn;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World;

public static class ModelDataLoader
{
    public static List<ModelData> LoadFromToml(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"TOML file not found: {filePath}");
        }

        var toml = Toml.ToModel<RootModel>(File.ReadAllText(filePath));
        var models = new List<ModelData>();

        foreach (var model in toml.Models)
        {
            models.Add(new ModelData(model));
        }

        return models;
    }
}

