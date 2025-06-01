using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;

namespace Opengl_virtual_tour_with_Raylib.Modules.Audio
{
    public class FootstepManager
    {
        private readonly List<ModelData> _sceneModels;
        private readonly Sound[] _footstepSounds;
        private readonly Sound[] _grassSounds;
        private readonly Sound _runSound;
        private readonly Sound _runGrassSound; // Nuevo sonido agregado

        private Vector3 _lastPlayerPos;
        private float _stepCooldown = 0.0f;
        private float _cooldownTime = 0.4f;

        public FootstepManager(List<ModelData> models)
        {
            _sceneModels = models;

            // Cargar sonidos de pasos normales
            _footstepSounds = new Sound[]
            {
                LoadSound("Assets/SoundEffects/Footstep1.mp3"),
                LoadSound("Assets/SoundEffects/Footstep2.mp3"),
                LoadSound("Assets/SoundEffects/Footstep3.mp3")
            };

            // Cargar sonidos de pasto
            _grassSounds = new Sound[]
            {
                LoadSound("Assets/SoundEffects/Rustling-grass1.mp3"),
                LoadSound("Assets/SoundEffects/Rustling-grass2.mp3"),
                LoadSound("Assets/SoundEffects/Rustling-grass3.mp3")
            };

            // Cargar sonidos de correr
            _runSound = LoadSound("Assets/SoundEffects/RunStep.mp3");
            _runGrassSound = LoadSound("Assets/SoundEffects/running-grass.mp3"); //  Agregado
        }

        public void Update(Vector3 playerPosition, bool isRunning = false)
        {
            _stepCooldown -= GetFrameTime();
            float movement = Vector3.Distance(playerPosition, _lastPlayerPos);

            // Detener sonidos si se deja de correr o se detiene el movimiento
            if (movement <= 0.01f || !isRunning)
            {
                if (IsSoundPlaying(_runSound)) StopSound(_runSound);
                if (IsSoundPlaying(_runGrassSound)) StopSound(_runGrassSound);
            }

            if (movement > 0.05f && _stepCooldown <= 0.0f)
            {
                Vector3 footPos = new Vector3(playerPosition.X, 0.0f, playerPosition.Z);

                foreach (var model in _sceneModels)
                {
                    if (IsPointInsideBox(footPos, GetModelBoundingBox(LoadModel(model.ModelPath))))
                    {
                        if (model.AssetName.Equals("tile"))
                        {
                            // Suelo de pasto
                            if (isRunning)
                            {
                                if (!IsSoundPlaying(_runGrassSound))
                                    PlaySound(_runGrassSound);
                                _cooldownTime = 0.25f;
                            }
                            else
                            {
                                int index = GetRandomValue(0, _grassSounds.Length - 1);
                                PlaySound(_grassSounds[index]);
                                _cooldownTime = 0.45f;
                            }
                        }
                        else
                        {
                            // Suelo de tierra
                            if (isRunning)
                            {
                                if (!IsSoundPlaying(_runSound))
                                    PlaySound(_runSound);
                                _cooldownTime = 0.25f;
                            }
                            else
                            {
                                int index = GetRandomValue(0, _footstepSounds.Length - 1);
                                PlaySound(_footstepSounds[index]);
                                _cooldownTime = 0.4f;
                            }
                        }

                        _stepCooldown = _cooldownTime;
                        break;
                    }
                }

                _lastPlayerPos = playerPosition;
            }
        }

        public void Unload()
        {
            foreach (var s in _footstepSounds)
                UnloadSound(s);

            foreach (var s in _grassSounds)
                UnloadSound(s);

            UnloadSound(_runSound);
            UnloadSound(_runGrassSound); //  Agregado
        }

        private bool IsPointInsideBox(Vector3 point, BoundingBox box)
        {
            return (point.X >= box.Min.X && point.X <= box.Max.X) &&
                   (point.Y >= box.Min.Y && point.Y <= box.Max.Y) &&
                   (point.Z >= box.Min.Z && point.Z <= box.Max.Z);
        }
    }
}
