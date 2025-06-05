using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Hitboxes; 
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Camera
{
    // Modes for the camera
    public enum CameraModeType
    {
        Tourist, // The height is fixed, it has few movements. The speed is lower 
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
        public static CameraModeType Mode { get; private set; } = CameraModeType.Tourist;
        // The camera's mode by default is tourist
        
        //<Temporal>
        private static float _velocityY;
        private const float Gravity=0.015f;
        private const float MaxFallSpeed=1.0f;
        //</Temporal>
        
        public const float HitBoxSize=0.1f;
        
        static CharacterCamera3D()
        {
            Camera = new Camera3D()
            {
                Position = new Vector3(3, GroundY, 3),

                Target = new Vector3(0, 0, 0),

                Up = Vector3.UnitY,

                FovY = 45.0f,
                
                Projection = CameraProjection.Perspective
            };
        }
        
        //<Temporal>
        // Acts like gravity and adjust the camera's height of the camera to the ground (OBBs)
        
        private static void AdaptCameraToGroundWithGravity(List<Hitbox> sueloHitboxes, bool isMoving)
        {
            // Just applies the gravity and raycast if there's movement or the camera is falling
            if (isMoving || _velocityY != 0f)
            {
                // Gravity
                _velocityY -= Gravity;
                if (_velocityY < -MaxFallSpeed)
                    _velocityY = -MaxFallSpeed;

                Camera.Position = new Vector3(Camera.Position.X, Camera.Position.Y + _velocityY, Camera.Position.Z);

                // Checking the ground
                Vector3 origen = Camera.Position;
                Ray ray = new Ray(new Vector3(origen.X, origen.Y, origen.Z), new Vector3(0, -1, 0));
                float minDist = float.MaxValue;
                float? alturaSuelo = null;

                foreach (var hitbox in sueloHitboxes)
                {
                    var col = hitbox.Box.GetRayCollision(ray);
                    if (col.Hit && col.Point.Y < origen.Y && col.Distance < minDist)
                    {
                        minDist = col.Distance;
                        alturaSuelo = col.Point.Y;
                    }
                }

                if (alturaSuelo.HasValue && Camera.Position.Y <= alturaSuelo.Value + GroundY)
                {
                    Camera.Position = new Vector3(origen.X, alturaSuelo.Value + GroundY, origen.Z);
                    _velocityY = 0f;
                }
            } 
        }
        //</Temporal>
        
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
                    // There's no collision, moves the camera
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
        private static void HandleTouristModeInput(List<Hitbox> obbHitboxes)
        {
            Vector3 forward=Vector3.Normalize(Camera.Target - Camera.Position);

            forward.Y = 0;
            
            Vector3 right =Vector3.Cross(forward, Camera.Up);

            right.Y = 0;

            Vector3 movement = Vector3.Zero;
            bool isMoving = false;

            if (IsKeyDown(KeyboardKey.W))
            {
                //Raylib.CameraMoveForward(ref Camera, Speed,true);   
                movement += forward*Speed;
                isMoving = true;

            }
            
            if (IsKeyDown(KeyboardKey.S))
            {
                //Raylib.CameraMoveForward(ref Camera, Speed,true);   
                movement -= forward*Speed;
                isMoving = true;
            }
                
            if (IsKeyDown(KeyboardKey.D))
            {
                //Raylib.CameraMoveForward(ref Camera, Speed,true);   
                movement += right*Speed;
                isMoving = true;
            }
            
            if (IsKeyDown(KeyboardKey.A))
            {
                //Raylib.CameraMoveForward(ref Camera, Speed,true);   
                movement -= right*Speed;
                isMoving = true;
            }
            
            if (IsKeyDown(KeyboardKey.LeftShift))
                movement *= 6.0f;
            
            
            //bool isMoving = movement.Length() > 0.0f;
            //bool isRunning = isMoving && Raylib.IsKeyDown(KeyboardKey.LeftShift);

            /*if (Mode == CameraModeType.Tourist && isMoving && FootstepAudio != null)
                FootstepAudio.Update(Camera.Position, isRunning);*/
            
            TryMoveCamera(Camera.Position+movement, obbHitboxes);
            
            if (_velocityY != 0f) isMoving = true;
            
            AdaptCameraToGroundWithGravity(obbHitboxes, isMoving);
            HandleMouseRotation();
        }

        public static void UpdateMyCamera(HitboxLoader hitboxLoader, CameraMode camMode)
        {
            //Choose a mode to use the camera 
            //By default is in Tourist mode
            if (IsKeyPressed(KeyboardKey.One)) 
                Mode = CameraModeType.Tourist;// Tourist Mode

            if (IsKeyPressed(KeyboardKey.Two)) 
                Mode = CameraModeType.Free;// Free mode

            // Movement modes
            if (Mode == CameraModeType.Tourist)
            {
                HandleTouristModeInput(hitboxLoader.Cajas);
            }
            else
            {
                // Update CharacterCamera3D position and hitbox
                UpdateCamera(ref Camera, camMode);
            }
        }
        
    }
}