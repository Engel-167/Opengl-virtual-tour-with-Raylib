using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Buildings;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Props;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Roads;
using Opengl_virtual_tour_with_Raylib.Modules.Camera;
using Opengl_virtual_tour_with_Raylib.Modules.Lighting;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Scenes;

public class MainScene (byte id, string windowTitle): SceneObject(id, windowTitle)
{
    private Buildings? _buildings;
    private Roads? _roads;
    private Props? _props;

    private CameraMode _camMode;
    private bool _cameraControlEnabled;

    private List<World3DObjects>? _worldObjects;
    private bool _hitboxEnabled;

    public override void InitScene()
    {
        _camMode = CameraMode.Custom;
        
        _buildings = new Buildings("ConfigurationFiles/DATA/BuildingsDATA.toml");
        _roads = new Roads("ConfigurationFiles/DATA/RoadsDATA.toml");
        _props = new Props("ConfigurationFiles/DATA/PropsDATA.toml");
        
        //InitializeWorld();
        _worldObjects = new List<World3DObjects>();
        _worldObjects.AddRange(_buildings);
        _worldObjects.AddRange(_roads);
        _worldObjects.AddRange(_props);
            
        ShadowMap.Init(_worldObjects);
        
        _hitboxEnabled = true;
    }

    public override int UpdateScene()
    {
            //BeginDrawing();
            ClearBackground(Color.SkyBlue);
                
            // Dibujar el hitbox de la cámara
            //DrawBoundingBox(CharacterCamera3D.HitBox, Color.Blue);
                
            // Change the camera Target when the middle mouse button and the F key is pressed
            if (IsMouseButtonDown(MouseButton.Middle)||IsKeyDown(KeyboardKey.F))
            {
                CharacterCamera3D.Camera.Target = new Vector3(0,0,0); // Make the camera look at the cube
            }
                
            // Start capturing the mouse
            if (IsMouseButtonDown(MouseButton.Left))
            {
                _camMode = CameraMode.Free;
                DisableCursor();
            }

            if (IsKeyPressed(KeyboardKey.Y))
            {
                    //ClearConfigFlags(ConfigFlags.Msaa4xHint);
                    ClearWindowState(ConfigFlags.Msaa4xHint);
                    CloseWindow();
            }
                
            if (IsKeyPressed(KeyboardKey.X))
                SetConfigFlags(ConfigFlags.Msaa4xHint);
                
            if (!_cameraControlEnabled && IsMouseButtonPressed(MouseButton.Left))
            {
                _cameraControlEnabled = true;
                DisableCursor(); // Captura del mouse
            }
            
            // Permitir cambio de modo solo si ya se activó la cámara
            if (_cameraControlEnabled)
            {
                
                //Choose a mode to use the camera 
                //By default is in Tourist mode
                if (IsKeyPressed(KeyboardKey.One)) 
                    CharacterCamera3D.Mode = CameraModeType.Tourist;// Movimiento del modo Turista

                if (IsKeyPressed(KeyboardKey.Two)) 
                    CharacterCamera3D.Mode = CameraModeType.Free;// Movimiento del modo Libre

                // Movimiento según el modo
                if (CharacterCamera3D.Mode == CameraModeType.Tourist)
                {
                    if (_buildings != null)
                        CharacterCamera3D.HandleTouristModeInput(_buildings.ModelDatas);
                }
                else
                {
                    // Update CharacterCamera3D position and hitbox
                    UpdateCamera(ref CharacterCamera3D.Camera, _camMode);
                }

                // Actualizar posición y restricciones
                CharacterCamera3D.UpdateHitBox();
                CharacterCamera3D.ApplyCameraConstraints();
            }

            // if key M is preced then stop updating the shadowmap and if is pressed again then enable the shadowmap update
            if (IsKeyPressed(KeyboardKey.M))
            {
                ShadowMap.Enabled = !ShadowMap.Enabled;

                if (ShadowMap.Enabled)
                {
                    if (_worldObjects != null) ShadowMap.Init(_worldObjects);
                }
            }   
            
            ShadowMap.Update();
            
            // Begin 3D mode
            BeginMode3D(CharacterCamera3D.Camera);

            if (_worldObjects != null) Render3DModels(_worldObjects);
            //DrawSphere(ShadowMap.GetLightCamPosition(), 1.0f, Yellow);

            if (IsKeyPressed(KeyboardKey.B))
            {
                _hitboxEnabled = !_hitboxEnabled;
            }
            
            if (_hitboxEnabled)
            {
                _buildings?.DrawHitBoxes();
            }
            
            // End 3D mode
            EndMode3D();

            // Draw UI
            DrawText("Colision False", 28, 10, 20, Color.Black);
            
            DrawText($@"
                Raylib GLTF 3D model Loading
                {GetFPS()} fps                
                Camera Pos: {CharacterCamera3D.Camera.Position}
                CameraBox: MIN-{CharacterCamera3D.HitBox.Min} MAX-{CharacterCamera3D.HitBox.Max}
                Hitbox Enabled = {((_hitboxEnabled)?"Yes":"No")} (Press B to toggle)",-100,10,20,Color.Black);
                
            DrawText($@"Current Mode < {CharacterCamera3D.Mode} >", 200, 10, 20, Color.Black);
            DrawText($"Enable shadows: {ShadowMap.Enabled} (Press M to toggle)", 200, 50, 20, Color.Red);
            // End drawing
            //EndDrawing();
        return 0;   
    }

    public override void KillScene()
    {
        if (_worldObjects != null) Clear3DModels(_worldObjects);
        ShadowMap.UnloadShadowmapRenderTexture();
    }
}