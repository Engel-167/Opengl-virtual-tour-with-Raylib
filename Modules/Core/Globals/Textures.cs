using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;

public static class Textures
{
    // UI-UX Textures
    // cream with brown borders style
    public static Texture2D BasicPanel = Raylib.LoadTexture("Assets/UI-UX/kenney_ui-pack-adventure/PNG/Double/panel_brown_damaged.png");
    public static Texture2D BasicPanelFocus = Raylib.LoadTexture("Assets/UI-UX/kenney_ui-pack-adventure/PNG/Double/panel_brown_corners_b.png");

    // blue X button style
    public static Texture2D BlueExitButton = Raylib.LoadTexture("Assets/UI-UX/kenney_ui-pack-adventure/PNG/Double/checkbox_grey_cross.png");
    public static Texture2D BlueExitButtonFocus = Raylib.LoadTexture("Assets/UI-UX/kenney_ui-pack-adventure/PNG/Double/checkbox_grey_checked.png");

    // Github icon
    public static Texture2D GithubIcon = Raylib.LoadTexture("Assets/UI-UX/FlatIcons/Github/githubx64.png");
    public static Texture2D GithubIconHover = Raylib.LoadTexture("Assets/UI-UX/FlatIcons/Github/githubx64White.png");
    
    // Brown with gray focus style
    public static Texture2D BrownPanel = Raylib.LoadTexture("Assets/UI-UX/kenney_ui-pack-adventure/PNG/Double/panel_brown_dark.png");
    public static Texture2D BrownPanelFocus = Raylib.LoadTexture("Assets/UI-UX/kenney_ui-pack-adventure/PNG/Double/panel_brown_dark_corners_a.png");
}