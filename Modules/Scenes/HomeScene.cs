using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Buildings;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Roads;
using Opengl_virtual_tour_with_Raylib.Modules.UI_UX;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Scenes;

/// <summary>This class contains the structure and functioning of the HomeScene,
/// which is the one that receives the User and shows to him the menu with different options
/// such as Play, Settings and Exit</summary>

public class HomeScene(byte id, string windowTitle) : SceneObject(id, windowTitle)
{
    ///<summary>Variable to play the sound of the button</summary> 
    private Sound _fxButton;
    /// <summary>Background Music</summary>
    private Music _bgMusic;
    /// <summary>Camera needed for the 3D background</summary>
    private Camera3D _camera;
    /// <summary> List of the worldObjects that will be drawn in the background</summary>
    private List<World3DObjects>? _worldObjects;
    
    private Button? _startButton;
    
    public bool SwapScene;
    public override void InitScene()
    {
        
        InitAudioDevice();      // Initialize audio device
        
        //InitializeWorld();
        Buildings buildings = new Buildings("ConfigurationFiles/DATA/BuildingsDATA.toml");
        Roads roads = new Roads("ConfigurationFiles/DATA/RoadsDATA.toml");
        
        _worldObjects = new List<World3DObjects>();
        _worldObjects.AddRange(buildings);
        _worldObjects.AddRange(roads);

        _bgMusic = LoadMusicStream("Assets/Music/Sketchbook 2024-10-30.ogg"); // Assets/Music/Sketchbook 2024-10-30.ogg
        _fxButton = LoadSound("Assets/UI-UX/Buttons/buttonfx.wav");   // Load button sound
        
        _camera = new Camera3D
        {
            Position = new Vector3(5, 3, 5),

            Target = new Vector3(0, 0, 0),

            Up = Vector3.UnitY,

            FovY = 45.0f,
                
            Projection = CameraProjection.Perspective
            
        };
        
        PlayMusicStream(_bgMusic);
        
        //testing buttons

        _startButton = new Button(LoadTexture("Assets/UI-UX/kenney_ui-pack-adventure/PNG/Double/panel_brown_damaged.png"),LoadTexture("Assets/UI-UX/kenney_ui-pack-adventure/PNG/Double/panel_brown_corners_b.png"),new Vector2(GetScreenWidth()/2.0f - 125f, GetScreenHeight()/2.0f - 50f) , 250,100)
            {
                Text = "Iniciar",
                Font = GetFontDefault(),
                FontSize = 32f,
                FontSpacing = 2f,
            };
        _startButton.Event += (_, _) =>
        {
            PlaySound(_fxButton);
            SwapScene = true;
        };
    }
    
    public override int UpdateScene()
    {
        UpdateMusicStream(_bgMusic);
        // Update
        //----------------------------------------------------------------------------------
        BeginDrawing();
        //----------------------------------------------------------------------------------
            
        // Here starts the Background rendering
        ClearBackground(Color.SkyBlue);
        
        UpdateCamera(ref _camera, CameraMode.Orbital);
        
        BeginMode3D(_camera);
        
        if (_worldObjects != null) Render3DModels(_worldObjects);
        
        EndMode3D();

        // Draw the Start Button
        if (_startButton != null) _startButton.Draw();
        //----------------------------------------------------------------------------------
        return 0;
    }

    public override void KillScene()
    {
        // De-Initialization
        //--------------------------------------------------------------------------------------
        UnloadSound(_fxButton);  // Unload sound
        UnloadMusicStream(_bgMusic); // Unload music stream

        CloseAudioDevice();     // Close audio device
    }
}