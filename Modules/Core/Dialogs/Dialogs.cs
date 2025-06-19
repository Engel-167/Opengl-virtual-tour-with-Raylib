namespace Opengl_virtual_tour_with_Raylib.Modules.Core.Dialogs;

public class Dialogs
{
    public string Barracks { get; set; } = null!;
    public string Castle { get; set; } = null!;
    public string Crane { get; set; } = null!;
    public string Mine { get; set; } = null!;
    public string Market { get; set; } = null!;

    public string GetDialog(string interactableObjectId)
    {
        return interactableObjectId switch
        {
            "Barracks" => Barracks,
            "Castle" => Castle,
            "Crane" => Crane,
            "Mine" => Mine,
            "Market" => Market,
            _ => String.Empty
        };
    }
}