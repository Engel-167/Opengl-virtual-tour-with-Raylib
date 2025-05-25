using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.Camera
{
    // Modes for the camera
    public enum CameraModeType
    {
        Tourist, // The height is fixed,it has few movements and the speed is lower 
        Free //More freedom, useful to look for the map
    }
    public static class CharacterCamera3D
    {
        private const float GroundY = 0.35f; //Minimum height for the camera

        private const float Speed = 0.03f; //Camera's speed
        
        private const float MouseSensitivity = 0.1f; //Mouse sensitivity 
        
        public static Camera3D Camera;
        
        private static Vector3 _lastPosition;
        
        private static float _pitch; // Cumulative vertical rotation 
        private static float _yaw; // Cumulative horizontal rotation
        
        public static CameraModeType Mode { get; set; } = CameraModeType.Tourist;
        // The camera's mode by default is tourist
        
        public static BoundingBox HitBox { get; private set; }
        private const float HitBoxSize=0.1f;
        
        static CharacterCamera3D()
        {
            Camera = new Camera3D()
            {
                Position = new Vector3(3, 1, 3),

                Target = new Vector3(0, 0, 0),

                Up = Vector3.UnitY,

                FovY = 45.0f,
                
                Projection = CameraProjection.Perspective
            };
            
            _lastPosition = Camera.Position;
            
        }
        
        // Method to update the HitBox only if the camera's position changes
        public static void UpdateHitBox()
        {
            if (Camera.Position != _lastPosition)
            {
                HitBox = new BoundingBox(
                    Camera.Position - new Vector3(HitBoxSize, HitBoxSize, HitBoxSize), // Rear bottom left corner
                    Camera.Position + new Vector3(HitBoxSize, HitBoxSize, HitBoxSize) // Front top right corner
                );
                _lastPosition = Camera.Position;
            }
        }
        
        private static void TryMoveCamera(Vector3 newPosition, List<ModelData> allModels) 
        {
            // Create the new tentative box
            var tentativeBox = new BoundingBox(
                newPosition - new Vector3(HitBoxSize, HitBoxSize, HitBoxSize),
                newPosition + new Vector3(HitBoxSize, HitBoxSize, HitBoxSize)
            );
            
            bool collided = false;
    
            foreach (var model in allModels)
            {
                if (Raylib.CheckCollisionBoxes(tentativeBox, model.BoundingBox))
                {
                    collided = true;
                    break;
                }
            }

            if (!collided)
            {
                Camera.Position = newPosition;
                UpdateHitBox();
                return;
            }

            // Try to move in the x-axis
            Vector3 tryX = new Vector3(newPosition.X, Camera.Position.Y, Camera.Position.Z);
            var boxX = new BoundingBox(
                tryX - new Vector3(HitBoxSize, HitBoxSize, HitBoxSize),
                tryX + new Vector3(HitBoxSize, HitBoxSize, HitBoxSize)
            );
        
            bool xCollided = false;
            
            foreach (var model in allModels)
            {
                if (Raylib.CheckCollisionBoxes(boxX, model.BoundingBox))
                {
                    xCollided = true;
                    break;
                }
            }

            // try to move in the z-axis
            Vector3 tryZ = new Vector3(Camera.Position.X, Camera.Position.Y, newPosition.Z);
            var boxZ = new BoundingBox(
                tryZ - new Vector3(HitBoxSize, HitBoxSize, HitBoxSize),
                tryZ + new Vector3(HitBoxSize, HitBoxSize, HitBoxSize)
            );
    
            bool zCollided = false;
        
            foreach (var model in allModels)
            {
                if (Raylib.CheckCollisionBoxes(boxZ, model.BoundingBox))
                {
                    zCollided = true;
                    break;
                }
            }

            // Apply the movement if there's some space
            if (!xCollided && zCollided)
            {
                Camera.Position = tryX;
                UpdateHitBox();
            }
        
            else if (xCollided && !zCollided)
            {
                Camera.Position = tryZ;
                UpdateHitBox();
            }
            // If both are free, we would move them before
            // If both are blocked, it doesn't move
        }
        
        
        public static void ApplyCameraConstraints()
        {
            Vector3 pos = Camera.Position;

            switch (Mode)
            {
                case CameraModeType.Tourist:
                    // Fixed height
                    Camera.Position = new Vector3(pos.X, GroundY, pos.Z);
                    break;

                case CameraModeType.Free:
                    //Without restrictions by now
                    /*
                     * // Minimum height
                    if (pos.Y < GroundY)
                        Camera.Position = new Vector3(pos.X, GroundY, pos.Z);
                    */
                    break;
            }
        }
        
        public static void HandleTouristModeInput(List<ModelData> allModels)
        {
            Vector3 forward=Vector3.Normalize(Camera.Target - Camera.Position);

            forward.Y = 0;
            
            Vector3 right =Vector3.Cross(forward, Camera.Up);

            right.Y = 0;

            Vector3 movement = Vector3.Zero;


            if (Raylib.IsKeyDown(KeyboardKey.W))
            {
                //Raylib.CameraMoveForward(ref Camera, Speed,true);    
                
                movement += forward*Speed;

            }
            
            if(Raylib.IsKeyDown(KeyboardKey.S))
                //Raylib.CameraMoveForward(ref Camera, -Speed,true);
                movement -= forward*Speed;
            
            if(Raylib.IsKeyDown(KeyboardKey.D))
                //Raylib.CameraMoveRight(ref Camera, Speed,true);
                movement += right*Speed;
            
            if(Raylib.IsKeyDown(KeyboardKey.A))
                //Raylib.CameraMoveRight(ref Camera, -Speed,true);
                movement -= right*Speed;
            
            if (Raylib.IsKeyDown(KeyboardKey.LeftShift))
                movement *= 6.0f;

            TryMoveCamera(Camera.Position+movement, allModels);
            
            // Takes the mouse's delta
            float mouseX = Raylib.GetMouseDelta().X * MouseSensitivity; 
            float mouseY = Raylib.GetMouseDelta().Y * MouseSensitivity;

            // Updates the rotation's angles (yaw and pitch)
            _yaw += mouseX;         // Horizontal rotation
            _pitch -= mouseY;       // Vertical rotation (inverted because the movement to up must be positive)

            // Limit the vertical angle to avoid gimbal lock
            _pitch = Math.Clamp(_pitch, -89.0f, 89.0f);

            // Calculates the new vector based on yaw and pitch
            Vector3 direction;
            direction.X = MathF.Cos(_pitch * (MathF.PI / 180.0f)) * MathF.Cos(_yaw * (MathF.PI / 180.0f));
            direction.Y = MathF.Sin(_pitch * (MathF.PI / 180.0f));
            direction.Z = MathF.Cos(_pitch * (MathF.PI / 180.0f)) * MathF.Sin(_yaw * (MathF.PI / 180.0f));

            // Updates the Camera's target
            Camera.Target = Camera.Position + Vector3.Normalize(direction);
        }
    }

}