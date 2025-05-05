using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Buildings;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.Camera
{
    //Posibles modos para la cámara
    public enum CameraModeType
    {
        Tourist, //Altura fija, movimientos limitados, menor velocidad
        Free //Mayor libertad, no se traspasa el suelo
    }
    public static class CharacterCamera3D
    {
        private const float GROUND_Y = 1.0f; //Altura mínima del suelo

        private const float SPEED = 0.05f; //Velocidad de la cámara
        
        public static Camera3D Camera;
        
        private static Vector3 _lastPosition;
        
        private static float pitch = 0.0f; // Rotación vertical acumulada
        private static float yaw = 0.0f; // Rotación horizontal acumulada
        
        public static CameraModeType Mode { get; set; } = CameraModeType.Tourist;
        //Por ahora fijamos el modo turista por defecto
        
        public static BoundingBox HitBox { get; private set; }
        private const float HitBoxSize=0.5f;
        
        
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
            HitBox = new BoundingBox(
                Camera.Position - new Vector3(HitBoxSize, HitBoxSize, HitBoxSize),
                Camera.Position + new Vector3(HitBoxSize, HitBoxSize, HitBoxSize)
            );
            
            /*
            if (Camera.Position != _lastPosition)
            {
                HitBox = new BoundingBox(
                    Camera.Position - new Vector3(0.1f, 0.1f, 0.1f), // Esquina inf izq trasera
                    Camera.Position + new Vector3(0.1f, 0.1f, 0.1f) // Esquina sup der delantera
                );
                _lastPosition = Camera.Position;
            }*/
        }
        
        public static void TryMoveCamera(Vector3 newPosition, List<ModelData> allModels)
        {
            // Crear caja tentativa en la nueva posición
            var tentativeBox = new BoundingBox(
                newPosition - new Vector3(HitBoxSize, HitBoxSize, HitBoxSize),
                newPosition + new Vector3(HitBoxSize, HitBoxSize, HitBoxSize)
            );

            // Verificar si colisiona con algún modelo
            foreach (var model in allModels)
            {
                if (Raylib.CheckCollisionBoxes(tentativeBox, model.BoundingBox))
                {
                    return; // Colisión detectada, no mover
                }
            }

            // Si no hay colisión, actualizar la posición
            Camera.Position = newPosition;
            UpdateHitBox();
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


        public static void HandleTouristModeInput(List<ModelData> allModels)
        {
            Vector3 forward=Vector3.Normalize(Camera.Target - Camera.Position);

            forward.Y = 0;
            
            Vector3 right =Vector3.Cross(forward, Camera.Up);

            right.Y = 0;

            Vector3 movement = Vector3.Zero;
            
            if(Raylib.IsKeyDown(KeyboardKey.W))
                movement += forward*SPEED;
            
            if(Raylib.IsKeyDown(KeyboardKey.S))
                movement -= forward*SPEED;
            
            if(Raylib.IsKeyDown(KeyboardKey.D))
                movement += right*SPEED;
            
            if(Raylib.IsKeyDown(KeyboardKey.A))
                movement -= right*SPEED;

            //Camera.Position += movement;
            //Camera.Target += movement;
            
            TryMoveCamera(Camera.Position+movement, allModels);
    
            // Capturar el delta del mouse
            float mouseX = Raylib.GetMouseDelta().X * 0.5f; // Ajuste de sensibilidad (horizontal)
            float mouseY = Raylib.GetMouseDelta().Y * 0.5f; // Ajuste de sensibilidad (vertical)

            // Actualizar los ángulos de rotación (yaw y pitch)
            yaw += mouseX;         // Rotación horizontal
            pitch -= mouseY;       // Rotación vertical (invertida para que el movimiento hacia arriba sea positivo)

            // Limitar el ángulo vertical para evitar gimbal lock
            pitch = Math.Clamp(pitch, -89.0f, 89.0f);

            // Calcular el nuevo vector de dirección basado en yaw y pitch
            Vector3 direction;
            direction.X = MathF.Cos(pitch * (MathF.PI / 180.0f)) * MathF.Cos(yaw * (MathF.PI / 180.0f));
            direction.Y = MathF.Sin(pitch * (MathF.PI / 180.0f));
            direction.Z = MathF.Cos(pitch * (MathF.PI / 180.0f)) * MathF.Sin(yaw * (MathF.PI / 180.0f));

            // Actualizar el objetivo de la cámara
            Camera.Target = Camera.Position + Vector3.Normalize(direction);
        }
    }

}