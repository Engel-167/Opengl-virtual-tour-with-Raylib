using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Hitboxes;
using Opengl_virtual_tour_with_Raylib.Modules.Audio;

using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.Camera
{
    // Modes for the camera
    public enum CameraModeType
    {
        Tourist, // The height is fixed, it has few movements, the speed is lower 
        Free //More freedom, useful to look for the map
    }
    public static class CharacterCamera3D
    {
        private const float GroundY = 0.35f; //Minimum height for the camera
        private const float Speed = 0.03f; //Camera's speed
        private const float MouseSensitivity = 0.1f; //Mouse sensitivity
        
        public static Camera3D Camera;
        private static float _pitch; // Cumulative vertical rotation 
        private static float _yaw; // Cumulative horizontal rotation
        public static CameraModeType Mode { get; set; } = CameraModeType.Tourist;
        // The camera's mode by default is tourist
        
        public const float HitBoxSize=0.1f;
        
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
        }
        
        //This method checks if it's possible to move the camera without trespassing the hitbox in the world
        //If a position it's not allowed, the method checks if we can move the camera around the object     
        
        private static void TryMoveCamera(Vector3 targetPosition, List<Hitbox> obbHitboxes)
        {
            float camRadius = HitBoxSize;
            Vector3 currentPosition = Camera.Position;
            Vector3 remainingMovement = targetPosition - currentPosition;
            const int maxIterations = 3;
            const float epsilon = 0.001f;

            for (int i = 0; i < maxIterations && remainingMovement.LengthSquared() > 1e-8f; i++)
            {
                Vector3 nextPosition = currentPosition + remainingMovement;
                bool collided = false;
                Vector3 collisionNormal = Vector3.Zero;

                // Looks for the first collision (just the first you found)
                foreach (var hitbox in obbHitboxes)
                {
                    if (hitbox.Box.CheckCollisionSphere(nextPosition, camRadius, out Vector3 normal))
                    {
                        collided = true;
                        collisionNormal = normal;
                        break;
                    }
                }

                if (!collided)
                {
                    // There's no collision, moves the total and it ends
                    currentPosition += remainingMovement;
                    Camera.Position = currentPosition;
                    return;
                }

                // Taking the movement over the tangent plane to the normal
                float dot = Vector3.Dot(remainingMovement, collisionNormal);

                // If the movement points out of the surface, do sliding
                if (dot < 0)
                {
                    // Removes the component to the normal (sliding)
                    Vector3 slideMovement = remainingMovement - dot * collisionNormal;
                    // Forwards to almost reach the surface (avoids penetration)
                    currentPosition += remainingMovement + collisionNormal * (-dot + epsilon);
                    remainingMovement = slideMovement;
                }
                else
                {
                    // Movement points outside or is tangent, it cancels out the rest of it
                    remainingMovement = Vector3.Zero;
                }
            }

            // If you reached here, moves the camera where it can
            Camera.Position = currentPosition;
        }
        
        //Method that blocks the Y-coordinate if the camera has the Tourist mode enabled
        public static void ApplyCameraConstraints()
        {
            switch (Mode)
            {
                case CameraModeType.Tourist:
                    // Fixed height
                    Vector3 pos = Camera.Position;
                    
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

        private static void HandleMouseRotation()
        {
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
        
        //This method is called everytime we need to perform a camera's movement using the keyboard and mouse
        public static void HandleTouristModeInput(List<ModelData> allModels, List<Hitbox> obbHitboxes)
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
            
            
            bool isMoving = movement.Length() > 0.0f;
            bool isRunning = isMoving && Raylib.IsKeyDown(KeyboardKey.LeftShift);

            // Only play footstep sounds when in Tourist mode (not in Free mode)
            if (Mode == CameraModeType.Tourist && isMoving && FootstepAudio != null)
                FootstepAudio.Update(Camera.Position, isRunning);



            TryMoveCamera(Camera.Position+movement, obbHitboxes);
            
            HandleMouseRotation();
        }
        
        public static Frustum GetCurrentFrustum(float aspect)
        {
            return FrustumCulling.CameraGetFrustum(Camera, aspect);
        }
        
        public static FootstepManager FootstepAudio;

    }

}