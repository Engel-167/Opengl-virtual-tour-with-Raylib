using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules.Core;

public class AudioManager
{
    public void Initialize()
    {
        Raylib.InitAudioDevice();
        //Raylib.SetMasterVolume(0.5f);
    }
    
    public void Kill()
    {
        Raylib.CloseAudioDevice();
    }
}