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
        Tourist, // The height is fixed, it has few movements and the speed is lower 
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
        
        //This method checks if it's possible to move the camera without trespassing the hitbox in the world
        //If a position it's not allowed, the method checks if we can move the camera around the object
       private static void TryMoveCamera(Vector3 newPosition, List<ModelData> allModels, List<Hitbox> obbHitboxes)
        {
            float camRadius = HitBoxSize;
            Vector3 movement = newPosition - Camera.Position;
            if (movement.LengthSquared() < 1e-8f) return;

            // 1. Finds collisions with OBBs and gets the normal of the collision (if it exists)
            bool collided = false;
            Vector3 collisionNormal = Vector3.Zero;
            float minDist = float.MaxValue;

            foreach (var hitbox in obbHitboxes)
            {
                if (hitbox.Box.CheckCollisionSphere(newPosition, camRadius, out Vector3 normal))
                {
                    float dist = Vector3.Distance(Camera.Position, hitbox.Box.Center);
                    if (dist < minDist)
                    {
                        collided = true;
                        minDist = dist;
                        collisionNormal = normal;
                    }
                }
            }

            // 2. Checks collisions with AABBs (BoundingBox)
            if (!collided)
            {
                var tentativeBox = new BoundingBox(
                    newPosition - new Vector3(HitBoxSize, HitBoxSize, HitBoxSize),
                    newPosition + new Vector3(HitBoxSize, HitBoxSize, HitBoxSize)
                );

                foreach (var model in allModels)
                {
                    if (Raylib.CheckCollisionBoxes(tentativeBox, model.BoundingBox))
                    {
                        collided = true;
                        break;
                    }
                }
            }

            // 3. If there's no collision, moves the camera normally
            if (!collided)
            {
                Camera.Position = newPosition;
                UpdateHitBox();
                return;
            }

            // 4. If there's collision with OBB, try to slide using the normal
            if (collisionNormal != Vector3.Zero)
            {
                // Computes the allowed movement (sliding): component tangent to the collision's plane
                Vector3 slideMovement = movement - Vector3.Dot(movement, collisionNormal) * collisionNormal;
                Vector3 slideTarget = Camera.Position + slideMovement;

                // Checks for collisions on the direction of sliding (with OBBs)
                bool slideCollided = false;
                foreach (var hitbox in obbHitboxes)
                {
                    if (hitbox.Box.CheckCollisionSphere(slideTarget, camRadius, out _))
                    {
                        slideCollided = true;
                        break;
                    }
                }
                // And with AABB models
                if (!slideCollided)
                {
                    var slideBox = new BoundingBox(
                        slideTarget - new Vector3(HitBoxSize, HitBoxSize, HitBoxSize),
                        slideTarget + new Vector3(HitBoxSize, HitBoxSize, HitBoxSize)
                    );
                    foreach (var model in allModels)
                    {
                        if (Raylib.CheckCollisionBoxes(slideBox, model.BoundingBox))
                        {
                            slideCollided = true;
                            break;
                        }
                    }
                }

                // If it can move, apply the change
                if (!slideCollided)
                {
                    Camera.Position = slideTarget;
                    UpdateHitBox();
                }
                // If it can not slide, doesn't change
            }
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
            
            // Detectar si se está moviendo
            bool isMoving = movement.Length() > 0.0f;
            bool isRunning = isMoving && Raylib.IsKeyDown(KeyboardKey.LeftShift);

            // Llamar a FootstepManager si está en movimiento
            if (isMoving && FootstepAudio != null)
                FootstepAudio.Update(Camera.Position, isRunning);


            TryMoveCamera(Camera.Position+movement, allModels, obbHitboxes);
            
            HandleMouseRotation();
        }
        
        public static Frustum GetCurrentFrustum(float aspect)
        {
            return FrustumCulling.CameraGetFrustum(Camera, aspect);
        }
        
        public static FootstepManager FootstepAudio;

    }

}