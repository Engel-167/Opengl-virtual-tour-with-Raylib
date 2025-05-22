using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;

public static class Textures
{
    //Textures
    //White
    public static Texture2D BasicPanel = Raylib.LoadTexture("Assets/UI-UX/kenney_ui-pack-adventure/PNG/Double/panel_brown_damaged.png");
    public static Texture2D BasicPanelFocus = Raylib.LoadTexture("Assets/UI-UX/kenney_ui-pack-adventure/PNG/Double/panel_brown_corners_b.png");

    public static Texture2D GithubIcon = Raylib.LoadTexture("Assets/UI-UX/FlatIcons/Github/github64px.png");
    public static Texture2D GithubIconHover = Raylib.LoadTexture("Assets/UI-UX/FlatIcons/Github/githubHover64px.png");
}