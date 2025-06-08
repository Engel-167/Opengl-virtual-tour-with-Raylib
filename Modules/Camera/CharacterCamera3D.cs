using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Hitboxes; 
using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Collections.Generic;

namespace Opengl_virtual_tour_with_Raylib.Modules.Camera
{
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

        private static float _velocityY;
        private const float Gravity = 0.015f;
        private const float MaxFallSpeed = 1.0f;

        public const float HitBoxSize = 0.1f;

        static CharacterCamera3D()
        {
            Camera = new Camera3D()
            {
                Position = new Vector3(5, GroundY, 7),
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
            if (isMoving || _velocityY != 0f)
            {
                _velocityY -= Gravity;
                if (_velocityY < -MaxFallSpeed)
                    _velocityY = -MaxFallSpeed;

                Camera.Position = new Vector3(Camera.Position.X, Camera.Position.Y + _velocityY, Camera.Position.Z);

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

        // <Temporal>
        // Algorithm inspired by Fauerby to "slide" with OBBs, using the swept sphere.
        // This method checks if it's possible to move the camera without trespassing the hitbox in the world
        //If a position it's not allowed, the method checks if we can move the camera around the object 
        private static void TryMoveCamera(Vector3 targetPosition, List<Hitbox> obbHitboxes)
        {
            float camRadius = HitBoxSize;
            Vector3 currentPosition = Camera.Position;
            Vector3 movement = targetPosition - currentPosition;
            const int maxIterations = 5;
            const float epsilon = 0.001f;

            for (int iter = 0; iter < maxIterations && movement.LengthSquared() > 1e-8f; iter++)
            {
                float nearestT = 1.0f;
                Vector3 nearestNormal = Vector3.Zero;
                bool foundCollision = false;

                foreach (var hitbox in obbHitboxes)
                {
                    // Convert the movement to the local space of the OBB
                    Quaternion invRot = Quaternion.Inverse(hitbox.Box.Rotation);
                    Vector3 localStart = Vector3.Transform(currentPosition - hitbox.Box.Center, invRot);
                    Vector3 localEnd = Vector3.Transform(currentPosition + movement - hitbox.Box.Center, invRot);
                    Vector3 localMovement = localEnd - localStart;

                    // We use the swept sphere against the local AABB
                    if (SweptSphereAabb(localStart, localEnd, camRadius, hitbox.Box.HalfExtents, out float t, out Vector3 localHitNormal))
                    {
                        if (t < nearestT)
                        {
                            nearestT = t;
                            // We transform the normal to the original world
                            nearestNormal = Vector3.TransformNormal(localHitNormal, Matrix4x4.CreateFromQuaternion(hitbox.Box.Rotation));
                            nearestNormal = Vector3.Normalize(nearestNormal);
                            foundCollision = true;
                        }
                    }
                }

                if (!foundCollision)
                {
                    currentPosition += movement;
                    Camera.Position = currentPosition;
                    return;
                }

                // Move before the contact point
                Vector3 moveToContact = movement * (nearestT - epsilon);
                currentPosition += moveToContact;

                // Sliding: Projects the remaining movement over the tangent plane to the collision's surface
                Vector3 remainingMovement = movement - moveToContact;
                float dot = Vector3.Dot(remainingMovement, nearestNormal);
                Vector3 slideMovement = remainingMovement - dot * nearestNormal;

                movement = slideMovement;

                if (movement.LengthSquared() < 1e-8f)
                    break;
            }
            Camera.Position = currentPosition;
        } 
        
        /// Swept sphere vs AABB (in the space of the OBB).
        private static bool SweptSphereAabb(Vector3 start, Vector3 end, float radius, Vector3 halfExtents, out float tHit, out Vector3 hitNormal)
        {
            // The AABB is center on the origin
            Vector3 boxMin = -halfExtents;
            Vector3 boxMax = halfExtents;
            Vector3 direction = end - start;
            tHit = 1.0f;
            hitNormal = Vector3.Zero;

            float tmin = 0.0f;
            float tmax = 1.0f;
            Vector3 potentialNormal = Vector3.Zero;

            for (int i = 0; i < 3; i++)
            {
                float startVal = (i == 0) ? start.X : (i == 1) ? start.Y : start.Z;
                float dirVal = (i == 0) ? direction.X : (i == 1) ? direction.Y : direction.Z;
                float minVal = (i == 0) ? boxMin.X : (i == 1) ? boxMin.Y : boxMin.Z;
                float maxVal = (i == 0) ? boxMax.X : (i == 1) ? boxMax.Y : boxMax.Z;

                // We make the box bigger for the sphere's radius 
                minVal -= radius;
                maxVal += radius;

                if (Math.Abs(dirVal) < 1e-8f)
                {
                    if (startVal < minVal || startVal > maxVal)
                        return false;
                }
                else
                {
                    float ood = 1.0f / dirVal;
                    float t1 = (minVal - startVal) * ood;
                    float t2 = (maxVal - startVal) * ood;
                    float normalSign = -Math.Sign(ood);

                    if (t1 > t2) { (t1, t2) = (t2, t1); normalSign = -normalSign; }

                    if (t1 > tmin)
                    {
                        tmin = t1;
                        potentialNormal = Vector3.Zero;
                        if (i == 0) potentialNormal.X = normalSign;
                        if (i == 1) potentialNormal.Y = normalSign;
                        if (i == 2) potentialNormal.Z = normalSign;
                    }
                    if (t2 < tmax) tmax = t2;
                    if (tmin > tmax) return false;
                }
            }

            tHit = tmin;
            hitNormal = potentialNormal;
            return tmin >= 0.0f && tmin <= 1.0f;
        }
        //</Temporal>

        private static void HandleMouseRotation()
        {
            // Takes the mouse's delta
            float mouseX = GetMouseDelta().X * MouseSensitivity; 
            float mouseY = GetMouseDelta().Y * MouseSensitivity;

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
            Vector3 forward = Vector3.Normalize(Camera.Target - Camera.Position);
            forward.Y = 0;
            Vector3 right = Vector3.Cross(forward, Camera.Up);
            right.Y = 0;

            Vector3 movement = Vector3.Zero;
            bool isMoving = false;

            if (IsKeyDown(KeyboardKey.W))
            {
                movement += forward * Speed;
                isMoving = true;
            }
            if (IsKeyDown(KeyboardKey.S))
            {
                movement -= forward * Speed;
                isMoving = true;
            }
            if (IsKeyDown(KeyboardKey.D))
            {
                movement += right * Speed;
                isMoving = true;
            }
            if (IsKeyDown(KeyboardKey.A))
            {
                movement -= right * Speed;
                isMoving = true;
            }
            if (IsKeyDown(KeyboardKey.LeftShift))
                movement *= 6.0f;

            TryMoveCamera(Camera.Position + movement, obbHitboxes);

            if (_velocityY != 0f) isMoving = true;

            AdaptCameraToGroundWithGravity(obbHitboxes, isMoving);
            HandleMouseRotation();
        }

        public static void UpdateMyCamera(HitboxLoader hitboxLoader, CameraMode camMode)
        {
            //Choose a mode to use the camera 
            //By default is in Tourist mode
            if (IsKeyPressed(KeyboardKey.One))
                Mode = CameraModeType.Tourist; //Tourist mode

            if (IsKeyPressed(KeyboardKey.Two))
                Mode = CameraModeType.Free; //Free mode

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