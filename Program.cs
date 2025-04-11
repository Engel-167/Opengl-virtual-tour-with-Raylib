using System.Numerics;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib
{
    class Program
    {
        static void Main()
        {
            // Initialize the window
            Raylib.InitWindow(800, 600, "3D Collision Detection Example");
            Raylib.ToggleFullscreen();
            Raylib.SetTargetFPS(60);

            // Define the camera
            Camera3D camera = new Camera3D
            {
                Position = new Vector3(0.0f, 2.0f, 10.0f),
                Target = new Vector3(0.0f, 2.0f, 0.0f),
                Up = new Vector3(0.0f, 1.0f, 0.0f),
                FovY = 45.0f,
                Projection = CameraProjection.Perspective
            };

            // Define a cube with a bounding box
            Vector3 cubePosition = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 cubeSize = new Vector3(2.0f, 2.0f, 2.0f);
            BoundingBox cubeBoundingBox = new BoundingBox(
                cubePosition - cubeSize / 2,
                cubePosition + cubeSize / 2
            );

            // Player's bounding box
            Vector3 playerSize = new Vector3(1.0f, 2.0f, 1.0f);
            BoundingBox playerBoundingBox;

            // Movement and mouse sensitivity
            float mouseSensitivity = 0.005f;
            float movementSpeed = 5.0f;

            Raylib.DisableCursor();

            while (!Raylib.WindowShouldClose())
            {
                float deltaTime = Raylib.GetFrameTime();

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
                if (Raylib.IsKeyDown(KeyboardKey.D)) movement += right;
                
                // Check if the middle mouse button is pressed
                if (Raylib.IsMouseButtonDown(MouseButton.Middle))
                {
                    camera.Target = cubePosition; // Make the camera look at the cube
                }

                // Normalize movement and apply speed
                if (movement != Vector3.Zero)
                {
                    movement = Vector3.Normalize(movement) * movementSpeed * deltaTime;

                    // Update player's bounding box
                    playerBoundingBox = new BoundingBox(
                        camera.Position + movement - playerSize / 2,
                        camera.Position + movement + playerSize / 2
                    );

                    // Check for collision
                    if (!Raylib.CheckCollisionBoxes(playerBoundingBox, cubeBoundingBox))
                    {
                        camera.Position += movement;
                        camera.Target += movement;
                    }
                }

                // Start drawing
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.RayWhite);

                // Begin 3D mode
                Raylib.BeginMode3D(camera);

                // Draw the cube
                Raylib.DrawCube(cubePosition, cubeSize.X, cubeSize.Y, cubeSize.Z, Color.Red);
                Raylib.DrawBoundingBox(cubeBoundingBox, Color.Green);

                // End 3D mode
                Raylib.EndMode3D();

                // Draw UI
                Raylib.DrawText("Use WASD to move, mouse to look around (press the middle mouse button to focus the cube)", 10, 10, 20, Color.DarkGray);

                // End drawing
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}