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

public class HomeScene(byte id, string windowTitle) : SceneObject(id,windowTitle)
{
    /// <summary>Background Music</summary>
    
    /// <summary>Camera needed for the 3D background</summary>
    private Camera3D _camera;
    /// <summary> List of the worldObjects that will be drawn in the background</summary>
    private List<World3DObjects>? _worldObjects;

    private HomeUi? _homeUi;
    
    public override void InitScene()
    {
        //InitializeWorld();
        Buildings buildings = new Buildings("ConfigurationFiles/DATA/BuildingsDATA.toml");
        Roads roads = new Roads("ConfigurationFiles/DATA/RoadsDATA.toml");
        
        _worldObjects = new List<World3DObjects>();
        _worldObjects.AddRange(buildings);
        _worldObjects.AddRange(roads);
        
        _camera = new Camera3D
        {
            Position = new Vector3(5, 3, 5),

            Target = new Vector3(0, 0, 0),

            Up = Vector3.UnitY,

            FovY = 45.0f,
                
            Projection = CameraProjection.Perspective
            
        };
        
        _homeUi = new HomeUi();
        
        Initialized = true;
    }
    
    public override void UpdateScene()
    {
        // Update
        //----------------------------------------------------------------------------------
        BeginDrawing();
        
            ClearBackground(Color.SkyBlue);
            
            UpdateCamera(ref _camera, CameraMode.Orbital);
            
            BeginMode3D(_camera);
            
                if (_worldObjects != null) Render3DModels(_worldObjects);
            
            EndMode3D();
            
            // Draw the Start Button
            if (!Core.Globals.Variables.IsSettingsMenuEnabled)
            {
                if (_homeUi != null) _homeUi.Draw();
                if (Core.Globals.Variables.SettingsMenu != null) Core.Globals.Variables.SettingsMenu.UpdateLayout();
            }
            else
            {
                if (Core.Globals.Variables.SettingsMenu != null) Core.Globals.Variables.SettingsMenu.Draw();
                if (_homeUi != null) _homeUi.UpdateLayout();
            }
            
        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public override void KillScene()
    {
        // De-Initialization
        //--------------------------------------------------------------------------------------
        if (_worldObjects != null)
            foreach (var obj in _worldObjects)
            {
                obj.Unload3DModels();
            }

        Initialized = false;
    }
}