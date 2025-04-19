using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Buildings;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Roads;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;

namespace Opengl_virtual_tour_with_Raylib
{
    static class Program
    {
        static void Main()
        {
            // Initialize the window
            InitWindow(800, 600, "3D virtual tour");
            ToggleFullscreen();
            SetTargetFPS(60);

            // Define the camera
            Camera3D camera = new Camera3D
            {
                Position = new Vector3(0.0f, 2.0f, 10.0f),
                Target = new Vector3(0.0f, 0.0f, 0.0f),
                Up = new Vector3(0.0f, 1.0f, 0.0f),
                FovY = 45.0f,
                Projection = CameraProjection.Perspective
            };

            // Player's bounding box
            //Vector3 playerSize = new Vector3(1.0f, 2.0f, 1.0f);

            // Movement and mouse sensitivity
            //float mouseSensitivity = 0.005f;
            //float movementSpeed = 5.0f;
            
            Buildings buildings = new Buildings("ConfigurationFiles/DATA/BuildingsDATA.toml");
            
            Roads roads = new Roads("ConfigurationFiles/DATA/RoadsDATA.toml");

            BoundingBox cameraBox;
            
            CameraMode camMode = CameraMode.Custom;

            while (!WindowShouldClose())
            {
                /*float deltaTime = Raylib.GetFrameTime();

                // Get mouse movement
                Vector2 mouseDelta = Raylib.GetMouseDelta();
                float yaw = -mouseDelta.X * mouseSensitivity;
                float pitch = -mouseDelta.Y * mouseSensitivity;

                // Rotate the camera target
                Vector3 forward = Vector3.Normalize(camera.Target - camera.Position);
                Vector3 right = Vector3.Cross(forward, camera.Up);
                forward = Vector3.Transform(forward, Matrix4x4.CreateFromAxisAngle(camera.Up, yaw));
                forward = Vector3.Transform(forward, Matrix4x4.CreateFromAxisAngle(right, pitch));
                camera.Target = camera.Position + forward;

                // Movement controls
                Vector3 movement = Vector3.Zero;
                if (Raylib.IsKeyDown(KeyboardKey.W)) movement += forward;
                if (Raylib.IsKeyDown(KeyboardKey.S)) movement -= forward;
                if (Raylib.IsKeyDown(KeyboardKey.A)) movement -= right;
                if (Raylib.IsKeyDown(KeyboardKey.D)) movement += right;*/
                
                // Check if the middle mouse button is pressed
                if (IsMouseButtonDown(MouseButton.Middle)||IsKeyDown(KeyboardKey.F))
                {
                    camera.Target = new Vector3(0,0,0); // Make the camera look at the cube
                }
                
                if (IsMouseButtonDown(MouseButton.Left))
                {
                    camMode = CameraMode.Free;
                    DisableCursor();
                }
                
                // Update the camera's bounding box based on its new position
                cameraBox = new BoundingBox(
                    camera.Position - new Vector3(0.1f, 0.1f, 0.1f), // Min corner
                    camera.Position + new Vector3(0.1f, 0.1f, 0.1f)  // Max corner
                );
                
                UpdateCamera(ref camera, camMode);

                /*// Normalize movement and apply speed
                if (movement != Vector3.Zero)
                {
                    movement = Vector3.Normalize(movement) * movementSpeed * deltaTime;

                    // Update player's bounding box
                    var playerBoundingBox = new BoundingBox(
                        camera.Position + movement - playerSize / 2,
                        camera.Position + movement + playerSize / 2
                    );
                }*/

                // Start drawing
                BeginDrawing();
                ClearBackground(Blue);

                // Begin 3D mode
                BeginMode3D(camera);
                buildings.Draw3DModels();
                roads.Draw3DModels();
                // End 3D mode
                EndMode3D();

                // Draw UI
                DrawText("Colision False", 28, 10, 20, Black);
            
                DrawText($@"
                Raylib GLTF 3D model Loading
                {GetFPS()} fps
                Campera Pos: {camera.Position}
                CameraBox: MIN-{cameraBox.Min} MAX-{cameraBox.Max}",-100,10,20,Color.Black);
                // End drawing
                EndDrawing();
            }

            CloseWindow();
        }
    }
}