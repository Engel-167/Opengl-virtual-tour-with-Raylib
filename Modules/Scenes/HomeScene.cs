using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Buildings;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Roads;
using RayGUI_cs;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Scenes;

/// <summary>This class contains the structure and functioning of the HomeScene,
/// which is the one that receives the User and shows to him the menu with different options
/// such as Play, Settings and Exit</summary>

public class HomeScene(byte id, string windowTitle) : SceneObject(id, windowTitle)
{
    ///<summary>The Number of Frames that the Button images have</summary>> 
    private const int NumFrames = 3;
    /// <summary>Variable to store the mouse position</summary> 
    private Vector2 _mousePoint;
    /// <summary>Variable to store the button position</summary> 
    private Rectangle _btnBounds;
    /// <summary>Variable to keep track of the button</summary>
    private int _btnState;
    ///<summary>Variable to play the sound of the button</summary> 
    private Sound _fxButton;
    /// <summary>Background Music</summary>
    private Music _bgMusic;
    ///<summary>Variable to store the button frame</summary> 
    private Rectangle _sourceRec;
    /// <summary>Variable to Determine where the button frame begins</summary>
    private float _frameHeight;
    /// <summary> Variable for storing the button texture</summary>
    private Texture2D _button;
    /// <summary>Camera needed for the 3D background</summary>
    private Camera3D _camera;
    /// <summary> List of the worldObjects that will be drawn in the background</summary>
    private List<World3DObjects>? _worldObjects;

    public GuiContainer? Container;
    
    public bool swapScene = false;
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
        _button = LoadTexture("Assets/UI-UX/Buttons/Play-BTN-ES.png"); // Load button texture

        // Define frame rectangle for drawing
        _frameHeight = (float)_button.Height/NumFrames;
        _sourceRec = new Rectangle(0, 0, _button.Width, _frameHeight); //{ 0, 0, (float)button.Width, frameHeight };

        // Define button bounds on screen
        _btnBounds = new Rectangle(GetScreenWidth()/2.0f - _button.Width/2.0f, GetScreenHeight()/2.0f - (float)_button.Height/NumFrames/2.0f, _button.Width, _frameHeight ); //{ };

        _btnState = 0;               // Button state: 0-NORMAL, 1-MOUSE_HOVER, 2-PRESSED// Button action should be activated

        _mousePoint = new Vector2(0, 0);
        
        _camera = new Camera3D
        {
            Position = new Vector3(5, 3, 5),

            Target = new Vector3(0, 0, 0),

            Up = Vector3.UnitY,

            FovY = 45.0f,
                
            Projection = CameraProjection.Perspective
            
        };
        
        PlayMusicStream(_bgMusic);

        Button testButton = new Button((GetScreenWidth()/2) - 150, (GetScreenHeight()/2) + 80, 300,80, "Iniciar")
            {
                Event = () =>
                {
                    PlaySound(_fxButton);
                    swapScene = true;
                }
            };

        Container = new GuiContainer();
        Container.Add("testButton",testButton);
    }
    
    public override int UpdateScene()
    {
        UpdateMusicStream(_bgMusic);
        // Update
        //----------------------------------------------------------------------------------
        _mousePoint = GetMousePosition();
        bool btnAction = false;

        // Check button state
        if (CheckCollisionPointRec(_mousePoint, _btnBounds))
        {
            _btnState = IsMouseButtonDown(MouseButton.Left) ? 2 : 1;

            if (IsMouseButtonReleased(MouseButton.Left)) btnAction = true;
        }
        else _btnState = 0;

        if (btnAction)
        {
            PlaySound(_fxButton);
            Thread.Sleep(50);
            return 1;
        }
        
        
        BeginDrawing();
        // Calculate button frame rectangle to draw depending on button state
        _sourceRec.Y = _btnState*_frameHeight;
        //----------------------------------------------------------------------------------
            
        // Here starts the Background rendering
        ClearBackground(Color.SkyBlue);
        
        UpdateCamera(ref _camera, CameraMode.Orbital);
        
        BeginMode3D(_camera);
        
        if (_worldObjects != null) Render3DModels(_worldObjects);
        
        EndMode3D();

        DrawTextureRec(_button, _sourceRec, new Vector2(_btnBounds.X, _btnBounds.Y), Color.White); // Draw button frame
        if (Container != null) Container.Draw();
        
        //----------------------------------------------------------------------------------
        return 0;
    }

    public override void KillScene()
    {
        // De-Initialization
        //--------------------------------------------------------------------------------------
        UnloadTexture(_button);  // Unload button texture
        UnloadSound(_fxButton);  // Unload sound
        UnloadMusicStream(_bgMusic); // Unload music stream

        CloseAudioDevice();     // Close audio device
    }
}