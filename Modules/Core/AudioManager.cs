using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Core;

public class AudioManager
{
    // Use a Dictionary to map scene music indices to actual Music objects
    private Music _homeMusic;
    private Music _mainMusic;
    private Music _creditsMusic;

    private readonly Thread _musicThread;

    public AudioManager()
    {
        _musicThread = new(() =>
        {
            // Load all music once when the thread starts
            _homeMusic = LoadMusicStream("Assets/Music/Sketchbook 2024-10-14.ogg");
            _mainMusic = LoadMusicStream("Assets/Music/Sketchbook 2024-10-13.ogg");
            _creditsMusic = LoadMusicStream("Assets/Music/Sketchbook 2024-10-26.ogg");

            bool homeIsPlaying = true;
            bool mainIsPlaying = false;
            bool creditsIsPlaying = false;

            bool resume = false;
            
            PlayMusicStream(_homeMusic);
            
            while (!WindowShouldClose())
            {
                if (Variables.AppSettings.MusicEnabled)
                {
                    switch (Globals.Scenes.CurrentScene)
                    {
                        case Globals.Scenes.Scene.Home:
                        {
                            if (!homeIsPlaying)
                            {
                                mainIsPlaying = false;
                                creditsIsPlaying = false;
                                PlayMusicStream(_homeMusic);
                                homeIsPlaying = true;
                            }
                        
                            if (homeIsPlaying && !resume)
                            {
                                UpdateMusicStream(_homeMusic);
                            }
                            else
                            {
                                ResumeMusicStream(_homeMusic);
                                resume = false;
                            }
                        }
                            break;
                        case Globals.Scenes.Scene.Main:
                        {
                            if (!mainIsPlaying)
                            {
                                homeIsPlaying = false;
                                creditsIsPlaying = false;
                                PlayMusicStream(_mainMusic);
                                mainIsPlaying = true;
                            }
                        
                            if (mainIsPlaying && !resume)
                            {
                                UpdateMusicStream(_mainMusic);
                            }
                            else
                            {
                                ResumeMusicStream(_mainMusic);
                                resume = false;
                            }
                        }
                            break;
                        case Globals.Scenes.Scene.Credits:
                        {
                            if (!creditsIsPlaying)
                            {
                                homeIsPlaying = false;
                                mainIsPlaying = false;
                                PlayMusicStream(_creditsMusic);
                                creditsIsPlaying = true;
                            }

                            if (creditsIsPlaying && !resume)
                            {
                                UpdateMusicStream(_creditsMusic);
                            }
                            else
                            {
                                ResumeMusicStream(_creditsMusic);
                                resume = false;
                            }
                            
                        }
                            break;
                    }
                }
                else
                {
                    PauseMusic();
                    resume = true;
                }
            }
            
        });
    }

    public void Initialize()
    {
        InitAudioDevice();
        
        SetMasterVolume(Variables.AppSettings.MasterVolume);
        
        _musicThread.Start();
    }
    
    public void Kill()
    {
        CloseAudioDevice();
    }

    private void PauseMusic()
    {
        PauseMusicStream(_homeMusic);
        PauseMusicStream(_mainMusic);
        PauseMusicStream(_creditsMusic);
    }

    public static void CheckVolumeMaxTreshold()
    {
        //float currentMasterVolume = GetMasterVolume();
        if (Variables.AppSettings.MasterVolume > 1.00f)
        {
            Variables.AppSettings.MasterVolume = 1.00f;
        }
        
        if (Variables.AppSettings.MasterVolume < 0.00f)
        {
            Variables.AppSettings.MasterVolume = 0.00f;
        }
    }
}