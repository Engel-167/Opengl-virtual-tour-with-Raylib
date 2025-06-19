using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Hitboxes; 
using Raylib_cs;
using static Raylib_cs.Raylib;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;

namespace Opengl_virtual_tour_with_Raylib.Modules.Camera
{
    public enum CameraModeType
    {
        Tourist,
        Free
    }

    public static class CharacterCamera3D
    {
        private const float GroundY = 0.35f;
        private const float Speed = 0.03f;
        private const float MouseSensitivity = 0.1f;

        public static Camera3D Camera;
        private static float _pitch;
        private static float _yaw;
        public static CameraModeType Mode { get; private set; } = CameraModeType.Tourist;

        private static float _velocityY;
        private const float Gravity = 0.015f;
        private const float MaxFallSpeed = 1.0f;

        public const float HitBoxSize = 0.1f;
        
        //<Temporal>
        //</Temporal>
        
        static CharacterCamera3D()
        {
            Camera = new Camera3D
            {
                Position = new Vector3(0, GroundY, 1),
                Target = new Vector3(0, 0, 0),
                Up = Vector3.UnitY,
                FovY = 45.0f,
                Projection = CameraProjection.Perspective
            };
        }

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
        
        private static void FixCameraPenetration(List<Hitbox> obbHitboxes)
        {
            const int maxPushOuts = 5;
            const float pushEpsilon = 0.005f;
            float camRadius = HitBoxSize;

            for (int i = 0; i < maxPushOuts; i++)
            {
                Vector3 correction = Vector3.Zero;
                int count = 0;

                foreach (var hitbox in obbHitboxes)
                {
                    if (hitbox.Box.CheckCollisionSphere(Camera.Position, camRadius, out Vector3 normal))
                    {
                        correction += normal;
                        count++;
                    }
                }

                if (count == 0)
                    break; 

                correction = Vector3.Normalize(correction);
                Camera.Position += correction * pushEpsilon;
            }
        }
        
        private static void TryMoveCamera(Vector3 targetPosition, List<Hitbox> obbHitboxes)
        {
            float camRadius = HitBoxSize;
            Vector3 currentPosition = Camera.Position;
            Vector3 movement = targetPosition - currentPosition;
            const int maxIterations = 5;
            const float epsilon = 0.001f;
            List<Vector3> collisionNormals = new();

            for (int iter = 0; iter < maxIterations && movement.LengthSquared() > 1e-8f; iter++)
            {
                float nearestT = 1.0f;
                Vector3 nearestNormal = Vector3.Zero;
                bool foundCollision = false;

                foreach (var hitbox in obbHitboxes)
                {
                    Quaternion invRot = Quaternion.Inverse(hitbox.Box.Rotation);
                    Vector3 localStart = Vector3.Transform(currentPosition - hitbox.Box.Center, invRot);
                    Vector3 localEnd = Vector3.Transform(currentPosition + movement - hitbox.Box.Center, invRot);

                    if (SweptSphereAabb(localStart, localEnd, camRadius, hitbox.Box.HalfExtents, out float t, out Vector3 localHitNormal))
                    {
                        if (t < nearestT)
                        {
                            nearestT = t;
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

                Vector3 moveToContact = movement * (nearestT - epsilon);
                currentPosition += moveToContact;

                if (nearestNormal != Vector3.Zero)
                    collisionNormals.Add(nearestNormal);

                Vector3 remainingMovement = movement - moveToContact;
                
                for (int i = 0; i < collisionNormals.Count; i++)
                {
                    Vector3 n = collisionNormals[i];
                    float dot = Vector3.Dot(remainingMovement, n);
                    remainingMovement -= dot * n;
                }
                
                if (collisionNormals.Count == 2 && remainingMovement.LengthSquared() < 1e-8f)
                {
                    Vector3 edgeDir = Vector3.Cross(collisionNormals[0], collisionNormals[1]);
                    if (edgeDir.LengthSquared() > 1e-8f)
                    {
                        edgeDir = Vector3.Normalize(edgeDir);

                        float originalMoveDot = Vector3.Dot(movement, edgeDir);
                        Vector3 edgeMove = edgeDir * originalMoveDot * 0.5f;
                        
                        bool blocked = false;
                        foreach (var hitbox in obbHitboxes)
                        {
                            if (hitbox.Box.CheckCollisionSphere(currentPosition + edgeMove, camRadius, out _))
                            {
                                blocked = true;
                                break;
                            }
                        }
                        if (!blocked)
                        {
                            currentPosition += edgeMove;
                            Camera.Position = currentPosition;
                            return;
                        }
                    }
                }

                movement = remainingMovement;

                if (movement.LengthSquared() < 1e-8f)
                    break;
            }
            Camera.Position = currentPosition;
        }
  
        private static bool SweptSphereAabb(Vector3 start, Vector3 end, float radius, Vector3 halfExtents, out float tHit, out Vector3 hitNormal)
        {
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

        private static void HandleMouseRotation()
        {
            float mouseX = GetMouseDelta().X * MouseSensitivity; 
            float mouseY = GetMouseDelta().Y * MouseSensitivity;

            _yaw += mouseX;
            _pitch -= mouseY;
            _pitch = Math.Clamp(_pitch, -89.0f, 89.0f);

            Vector3 direction;
            direction.X = MathF.Cos(_pitch * (MathF.PI / 180.0f)) * MathF.Cos(_yaw * (MathF.PI / 180.0f));
            direction.Y = MathF.Sin(_pitch * (MathF.PI / 180.0f));
            direction.Z = MathF.Cos(_pitch * (MathF.PI / 180.0f)) * MathF.Sin(_yaw * (MathF.PI / 180.0f));

            Camera.Target = Camera.Position + Vector3.Normalize(direction);
        }
        
        private static void HandleTouristModeInput(List<Hitbox> obbHitboxes, List<Hitbox> obbGroundHitboxes )
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

            // 1. CORRECCIÓN DE PENETRACIÓN antes de moverse
            FixCameraPenetration(obbHitboxes);

            // 2. Sliding robusto
            TryMoveCamera(Camera.Position + movement, obbHitboxes);

            if (_velocityY != 0f) isMoving = true;

            AdaptCameraToGroundWithGravity(obbGroundHitboxes, isMoving);
            HandleMouseRotation();
        }

        public static void InteractionAnimDoors(List<Hitbox> doorsHitboxes)
        {
            float radius = 4.0f;
            bool isNearDoor = false;

            foreach (var hitbox in doorsHitboxes)
            {
                if (hitbox.Box.CheckCollisionSphere(Camera.Position, radius, out _))
                {
                    isNearDoor = true;
                    break;
                }
            }

            unsafe
            {
                if (isNearDoor)
                {
                    // Open: The animation goes forwards
                    Animations.animFrameCounter++;
                    if (Animations.animFrameCounter >= Animations.anims[0].FrameCount - 1)
                        Animations.animFrameCounter = Animations.anims[0].FrameCount - 1;
                }
                else
                {
                    // Close: The animation goes backwards
                    Animations.animFrameCounter--;
                    if (Animations.animFrameCounter < 0)
                        Animations.animFrameCounter = 0;
                }

                if (Variables.Buildings != null)
                {
                    UpdateModelAnimation(Variables.Buildings.GateModel, Animations.anims[0], Animations.animFrameCounter);
                    Console.WriteLine($"Frame count: {Animations.anims[0].FrameCount}");
                }
            }
        }
        
        public static void UpdateMyCamera(HitboxLoader hitboxLoader, HitboxLoader? groundLoader, HitboxLoader? doorsHitboxes,CameraMode camMode)
        {
            if (IsKeyPressed(KeyboardKey.One))
                Mode = CameraModeType.Tourist;
            if (IsKeyPressed(KeyboardKey.Two))
                Mode = CameraModeType.Free;

            if (Mode == CameraModeType.Tourist)
            {
                if (groundLoader?.Cajas != null)
                {
                    if (doorsHitboxes?.Cajas != null)
                    {
                        HandleTouristModeInput(hitboxLoader.Cajas, groundLoader.Cajas);

                        InteractionAnimDoors(doorsHitboxes.Cajas);
                    }
                }
            }
            else
            {
                UpdateCamera(ref Camera, camMode);
            }
        }
    }
}