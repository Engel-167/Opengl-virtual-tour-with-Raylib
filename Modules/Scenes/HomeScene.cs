using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.SkyBox;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
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

    private SkyBox? _skyBox;
    
    public override void InitScene()
    {
        _worldObjects = new List<World3DObjects>();
        if (Variables.Buildings != null) _worldObjects.AddRange(Variables.Buildings);
        if (Variables.Roads != null) _worldObjects.AddRange(Variables.Roads);

        _camera = new Camera3D
        {
            Position = new Vector3(5, 3, 5),

            Target = new Vector3(0, 0, 0),

            Up = Vector3.UnitY,

            FovY = 45.0f,
                
            Projection = CameraProjection.Perspective
            
        };
        
        _skyBox = new SkyBox();
        
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
            
                _skyBox?.Draw();
            
                if (_worldObjects != null) Render3DModels(_worldObjects);
            
            EndMode3D();
            
            // Draw the Start Button
            if (!Variables.IsSettingsMenuEnabled)
            {
                if (Variables.HomeUi != null) Variables.HomeUi.Draw();
                if (Variables.SettingsMenu != null) Variables.SettingsMenu.UpdateLayout();
            }
            else
            {
                if (Variables.SettingsMenu != null) Variables.SettingsMenu.Draw();
                if (Variables.HomeUi != null) Variables.HomeUi.UpdateLayout();
            }
            
        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    public override void KillScene()
    {
        // De-Initialization
        _skyBox?.Destroy();
        Initialized = false;
    }
}