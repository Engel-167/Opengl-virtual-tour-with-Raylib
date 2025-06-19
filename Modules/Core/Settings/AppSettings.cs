namespace Opengl_virtual_tour_with_Raylib.Modules.Core.Settings;

public sealed class AppSettings
{
    /* General -------------------------------------------------------- */
    public string Language { get; set; } = "EN";

    /* Video ---------------------------------------------------------- */
    public int ScreenWidth { get; set; }
    public int ScreenHeight { get; set; }
    public bool   Fullscreen     { get; set; } = true;
    public bool   ShadowsEnabled { get; set; } = true;

    /* Sound ---------------------------------------------------------- */
    public float MasterVolume   { get; set; } = 1.0f;
    public bool MusicEnabled { get; set; } = true;
}
