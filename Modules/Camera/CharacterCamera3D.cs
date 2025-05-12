using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
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
        private const float GroundY = 0.3f; //Altura mínima del suelo

        private const float Speed = 0.1f; //Velocidad de la cámara
        
        private const float MouseSensitivity = 0.1f;
        
        public static Camera3D Camera;
        
        private static Vector3 _lastPosition;
        
        private static float _pitch; // Rotación vertical acumulada
        private static float _yaw; // Rotación horizontal acumulada
        
        public static CameraModeType Mode { get; set; } = CameraModeType.Tourist;
        //Por ahora fijamos el modo turista por defecto
        
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
            //Solo se mueve si la posición se va a cambiar
            if (Camera.Position != _lastPosition)
            {
                HitBox = new BoundingBox(
                    Camera.Position - new Vector3(HitBoxSize, HitBoxSize, HitBoxSize), // Esquina inf izq trasera
                    Camera.Position + new Vector3(HitBoxSize, HitBoxSize, HitBoxSize) // Esquina sup der delantera
                );
                _lastPosition = Camera.Position;
            }
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
                    Camera.Position = new Vector3(pos.X, GroundY, pos.Z);
                    break;

                case CameraModeType.Free:
                    //Sin restricciones por el momento
                    /*
                     * // Altura mínima
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
            
            TryMoveCamera(Camera.Position+movement, allModels);
    
            
            
            // Capturar el delta del mouse
            float mouseX = Raylib.GetMouseDelta().X * MouseSensitivity; // Ajuste de sensibilidad (horizontal)
            float mouseY = Raylib.GetMouseDelta().Y * MouseSensitivity; // Ajuste de sensibilidad (vertical)

            // Actualizar los ángulos de rotación (yaw y pitch)
            _yaw += mouseX;         // Rotación horizontal
            _pitch -= mouseY;       // Rotación vertical (invertida para que el movimiento hacia arriba sea positivo)

            // Limitar el ángulo vertical para evitar gimbal lock
            _pitch = Math.Clamp(_pitch, -89.0f, 89.0f);

            // Calcular el nuevo vector de dirección basado en yaw y pitch
            Vector3 direction;
            direction.X = MathF.Cos(_pitch * (MathF.PI / 180.0f)) * MathF.Cos(_yaw * (MathF.PI / 180.0f));
            direction.Y = MathF.Sin(_pitch * (MathF.PI / 180.0f));
            direction.Z = MathF.Cos(_pitch * (MathF.PI / 180.0f)) * MathF.Sin(_yaw * (MathF.PI / 180.0f));

            // Actualizar el objetivo de la cámara
            Camera.Target = Camera.Position + Vector3.Normalize(direction);
        }
    }

}