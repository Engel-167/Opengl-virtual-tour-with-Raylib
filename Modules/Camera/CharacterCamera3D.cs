using System.Numerics;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.Camera
{
    //Posibles modos para la cámara
    //En el caso del Tourist, la altura y está fija en 1.0f, se mueve sobre las calles
    //En el Free, la restriccion es no sobrepasar el piso
    public enum CameraModeType
    {
        Tourist, 
        Free
    }
    public static class CharacterCamera3D
    {
        private const float GROUND_Y = 1.0f;
        
        public static Camera3D Camera;
        
        private static Vector3 _lastPosition;
        
        public static CameraModeType Mode { get; set; } = CameraModeType.Tourist;
        //Por ahora fijamos el modo turista
        
        public static BoundingBox HitBox { get; private set; }
        
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
        /*
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
        };*/
        
        // Method to update the HitBox only if the camera's position changes
        public static void UpdateHitBox()
        {
            if (Camera.Position != _lastPosition)
            {
                HitBox = new BoundingBox(
                    Camera.Position - new Vector3(0.1f, 0.1f, 0.1f), // Esquina inf izq trasera
                    Camera.Position + new Vector3(0.1f, 0.1f, 0.1f) // Esquina sup der delantera
                );
                _lastPosition = Camera.Position;
            }
        }
        
        public static void ApplyCameraConstraints()
        {
            Vector3 pos = Camera.Position;

            switch (Mode)
            {
                case CameraModeType.Tourist:
                    // Altura fija
                    Camera.Position = new Vector3(pos.X, GROUND_Y, pos.Z);
                    break;

                case CameraModeType.Free:
                    // Altura mínima
                    if (pos.Y < GROUND_Y)
                        Camera.Position = new Vector3(pos.X, GROUND_Y, pos.Z);
                    break;
            }
        }
        
    }

}