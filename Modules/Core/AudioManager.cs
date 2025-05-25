using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Core;

public class AudioManager
{
    // Use a Dictionary to map scene music indices to actual Music objects
    private readonly Dictionary<byte, Music> _loadedMusic = new Dictionary<byte, Music>();
    private Music _currentPlayingMusic; // Stores the currently playing music
    private byte _currentMusicIndex = byte.MaxValue; // Initialize with an invalid index

    private readonly Thread _musicThread;

    public AudioManager()
    {
        _musicThread = new(() =>
        {
            // Load all music once when the thread starts
            _loadedMusic.Add(0, LoadMusicStream("Assets/Music/Sketchbook 2024-10-13.ogg"));
            _loadedMusic.Add(1, LoadMusicStream("Assets/Music/Sketchbook 2024-10-14.ogg"));

            while (!WindowShouldClose())
            {
                // Check if the desired music has changed
                if (_currentMusicIndex != Variables.CurrentBgMusic)
                {
                    // Stop the old music if it was playing
                    if (_currentPlayingMusic.Stream.Buffer != IntPtr.Zero)
                    {
                        StopMusicStream(_currentPlayingMusic);
                    }

                    _currentMusicIndex = Variables.CurrentBgMusic;

                    // Start playing the new music
                    if (_loadedMusic.TryGetValue(_currentMusicIndex, out Music newMusic))
                    {
                        _currentPlayingMusic = newMusic;
                        PlayMusicStream(_currentPlayingMusic);
                    }
                }
                
                // Update the currently playing music stream
                if (_currentPlayingMusic.Stream.Buffer != IntPtr.Zero)
                {
                    UpdateMusicStream(_currentPlayingMusic);
                }
            }
            
            // Unload all loaded music when the window closes
            foreach (var music in _loadedMusic.Values)
            {
                UnloadMusicStream(music);
            }
        });
    }

    public void Initialize()
    {
        InitAudioDevice();
        _musicThread.Start();
    }
    
    public void Kill()
    {
        CloseAudioDevice();
    }
}