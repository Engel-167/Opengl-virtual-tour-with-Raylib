using System.Numerics;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.Camera;

    public static class CharacterCamera3D
    {
        private static Vector3 Position { get; set; } = new Vector3(3,1,3);
        private static Vector3 Target { get; set; } = new Vector3(0,0,0);
        private static Vector3 Up { get; set; } = Vector3.UnitY;
        private static float FovY { get; set; } = 45.0f;

        public static Camera3D Camera = new()
        {
            Position = Position,
            Target = Target,
            Up = Up,
            FovY = FovY,
            Projection = CameraProjection.Perspective
        };

        public static BoundingBox HitBox { get; private set; }

        private static Vector3 _lastPosition = Camera.Position;

        // Method to update the HitBox only if the camera's position changes
        public static void UpdateHitBox()
        {
            if (Camera.Position != _lastPosition)
            {
                HitBox = new BoundingBox(
                    Camera.Position - new Vector3(0.1f, 0.1f, 0.1f), // Min corner
                    Camera.Position + new Vector3(0.1f, 0.1f, 0.1f)  // Max corner
                );
                _lastPosition = Camera.Position;
            }
        }
    }    

