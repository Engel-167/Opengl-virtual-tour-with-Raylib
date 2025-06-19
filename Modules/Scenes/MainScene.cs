using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Hitboxes;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.SkyBox;
using Opengl_virtual_tour_with_Raylib.Modules.Camera;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Opengl_virtual_tour_with_Raylib.Modules.Lighting;
using Opengl_virtual_tour_with_Raylib.Modules.UI_UX.Elements;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Scenes;

public class MainScene (byte id, string windowTitle): SceneObject(id, windowTitle)
{
    private HitboxLoader? _hitboxLoader;
    private HitboxLoader? _groundLoader;
    private HitboxLoader? _doorLoader;
    
    private CameraMode _camMode;
    private bool _cameraControlEnabled;

    public static List<World3DObjects>? WorldObjects;
    private bool _hitboxEnabled;
    
    private static readonly Vector3 HitboxSize = new(CharacterCamera3D.HitBoxSize, CharacterCamera3D.HitBoxSize, CharacterCamera3D.HitBoxSize);
    
    private SkyBox? _skyBox;

    private Water _water = null!;
    
    private Panel _notifierPanel = null!;
    public override void InitScene()
    {
        _camMode = CameraMode.Custom;
        
        _hitboxLoader= new HitboxLoader("ConfigurationFiles/DATA/HitboxesDATA.toml");
        _groundLoader = new HitboxLoader("ConfigurationFiles/DATA/GroundDATA.toml");
        _doorLoader = new HitboxLoader("ConfigurationFiles/DATA/DoorsDATA.toml");
        
        //InitializeWorld();
        WorldObjects = new List<World3DObjects>();
        WorldObjects = new List<World3DObjects>();
        
        if (Variables.Buildings != null) WorldObjects.AddRange(Variables.Buildings);
        
        if (Variables.Roads != null) WorldObjects.AddRange(Variables.Roads);
        
        if (Variables.Props != null) WorldObjects.AddRange(Variables.Props);

        Render3DModels(WorldObjects);

        Console.WriteLine($"ShadowMap enabled 1 : {Variables.AppSettings.ShadowsEnabled}");
        if (Variables.AppSettings.ShadowsEnabled)
        {
            ShadowMap.Init(WorldObjects);
        }
        Console.WriteLine($"ShadowMap enabled 1 : {Variables.AppSettings.ShadowsEnabled}");
        
        _hitboxEnabled = true;
        
        _skyBox = new SkyBox();
        
        _water = new Water(new Vector3(10.5f, -0.5f, 2.0f),38f,7f);
        
        _notifierPanel = _notifierPanel = new Panel
        (
            Textures.Flag, new Vector2(GetScreenWidth() / 2f - 200, 5),
            400, 100,
            [55, 55, 55, 55]
        )
        {
            Text = Variables.Language.InteractString + " F",
            TextColor = Color.White,
            Font = Fonts.UbuntuM,
            FontSize = 32f,
            FontSpacing = 2f
        };
        
        Initialized = true;
    }
    
    private void UpdateLightFollow()
    {
        // Center the light camera on the player's XZ position to make shadows follow the player.
        Vector3 playerPos = CharacterCamera3D.Camera.Position;
        
        // The light camera will target the player's position on the ground plane.
        ShadowMap.LightCam.Target = playerPos with { Y = 0.0f };

        // The light camera is positioned away from the target, along the light's direction.
        // This ensures the shadow angle remains constant as the player moves.
        const float lightDistance = 35.0f; // Controls how far the light source is.
        ShadowMap.LightCam.Position = ShadowMap.LightCam.Target - (ShadowMap.GetLightDirection() * lightDistance);
    } 

    public override void UpdateScene()
    {
            // Start capturing the mouse
            if (IsMouseButtonDown(MouseButton.Left) && !Variables.IsSettingsMenuEnabled && !Variables.IsDialogBoxEnabled)
            {
                _camMode = CameraMode.Free;
                DisableCursor();
            }

            //Show Settings Ui
            if (IsKeyPressed(KeyboardKey.Escape))
            {
                Variables.IsSettingsMenuEnabled = !Variables.IsSettingsMenuEnabled;

                if (Variables.IsSettingsMenuEnabled)
                {
                    _cameraControlEnabled = false;
                    EnableCursor();    
                }
            }
            
            if (IsKeyPressed(KeyboardKey.F) && Variables.CanInteract)
            {
                Variables.IsDialogBoxEnabled = true;
                _cameraControlEnabled = false;
                EnableCursor();    
            }
            
            /*if (IsKeyPressed(KeyboardKey.F) && !Variables.CanInteract)
            {
                Variables.IsDialogBoxEnabled = false;
            }*/
            
            
                
            if (!_cameraControlEnabled && IsMouseButtonPressed(MouseButton.Left) && !Variables.IsSettingsMenuEnabled && !Variables.IsDialogBoxEnabled)
            {
                _cameraControlEnabled = true;
                DisableCursor();
            }
            
            // Change the camera mode only if the camera is already activated
            if (_cameraControlEnabled)
            {
                if (_hitboxLoader != null)
                    CharacterCamera3D.UpdateMyCamera(_hitboxLoader, _groundLoader,_doorLoader,_camMode);
            }

            // Toggle Shadows
            if (IsKeyPressed(KeyboardKey.M))
            {
                Variables.AppSettings.ShadowsEnabled = !Variables.AppSettings.ShadowsEnabled;
            
                if (Variables.AppSettings.ShadowsEnabled)
                {
                    if (WorldObjects != null) ShadowMap.Init(WorldObjects);
                }
                else
                {
                    ShadowMap.UnloadShadowmapRenderTexture();
                }
            }
            
            // ... (hitbox toggle logic)
            
            UpdateLightFollow();
            
            if (Variables.AppSettings.ShadowsEnabled)
            {
                ShadowMap.Update(WorldObjects);
            }
            
            BeginDrawing();
            
            // ... (rest of the drawing code)   
            
            // Enable the hitboxes drawing
            if (IsKeyPressed(KeyboardKey.B))
            {
                _hitboxEnabled = !_hitboxEnabled;
            }
            
            UpdateLightFollow();
            //Rlgl.DisableBackfaceCulling();
            //Rlgl.SetCullFace(0);
            if (Variables.AppSettings.ShadowsEnabled)
            {
                ShadowMap.Update(WorldObjects);   
            }
            else
            {
                ShadowMap.UnloadShadowmapRenderTexture();
            }
            
            BeginDrawing();
        
                ClearBackground(Color.SkyBlue);
                
                BeginMode3D(CharacterCamera3D.Camera);
            
                _skyBox?.Draw();
                
                if (_hitboxEnabled)
                {
                    _hitboxLoader?.DrawBoundingBoxes(Color.Blue);
                    _groundLoader?.DrawBoundingBoxes(Color.Red);
                    _doorLoader?.DrawBoundingBoxes(Color.DarkGreen);
                }
                
                if (WorldObjects != null) Render3DModels(WorldObjects);
                
                _water.Update();

                EndMode3D();
                
                DrawText("Collision False", 28, 10, 20, Color.Black);
            
                DrawText($@"
                Raylib GLTF 3D model Loading
                {GetFPS()} fps                
                Camera Pos: {CharacterCamera3D.Camera.Position}
                CameraBox: MIN-{CharacterCamera3D.Camera.Position-HitboxSize}
                 MAX-{CharacterCamera3D.Camera.Position+HitboxSize}
                Hitbox Enabled = {((_hitboxEnabled)?"Yes":"No")} (Press B to toggle)",-100,10,20,Color.Black);
                
                DrawText($@"Current Mode < {CharacterCamera3D.Mode} >", 200, 10, 20, Color.Black);
                DrawText($"Enable shadows: {Variables.AppSettings.ShadowsEnabled} (Press M to toggle)", 200, 50, 20, Color.Red);
                
                
                
                if (Variables.IsSettingsMenuEnabled && !Variables.IsDialogBoxEnabled)
                {
                    Variables.SettingsMenu?.Draw();
                }
                else
                {
                    Variables.IsSettingsMenuEnabled = false;
                }

                if (Variables.IsDialogBoxEnabled && !Variables.IsSettingsMenuEnabled)
                {
                    Variables.CanInteract = true;
                    Variables.DialogBox?.Draw();
                }

                if (Variables.CanInteract && !Variables.IsSettingsMenuEnabled)
                {
                    _notifierPanel.Text = Variables.Language.InteractString + " F";
                    _notifierPanel.Draw();
                }
                Console.WriteLine($"CanInteract: {Variables.CanInteract}");

                if (IsWindowResized())
                {
                    Variables.SettingsMenu?.UpdateLayout();
                    Variables.DialogBox?.UpdateLayout();
                    _notifierPanel.Position = new Vector2(GetScreenWidth() / 2f - 200, 5);
                }
            EndDrawing();
    }

    public override void KillScene()
    {
        _skyBox?.Destroy();
        _water.Kill();
        ShadowMap.UnloadShadowmapRenderTexture();
        
        Initialized = false;
    }
}