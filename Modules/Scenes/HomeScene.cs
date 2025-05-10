using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Scenes;

public class HomeScene(byte id, string windowTitle) : SceneObject(id, windowTitle)
{
    private const int NumFrames = 3;
    private Vector2 _mousePoint;
    private Rectangle _btnBounds;
    private int _btnState;
    private Sound _fxButton;
    private Rectangle _sourceRec;
    private float _frameHeight;
    private Texture2D _button;

    public override int UpdateScene()
    {
        // Update
        //----------------------------------------------------------------------------------
        _mousePoint = GetMousePosition();
        bool btnAction = false;

        // Check button state
        if (CheckCollisionPointRec(_mousePoint, _btnBounds))
        {
            if (IsMouseButtonDown(MouseButton.Left)) _btnState = 2;
            else _btnState = 1;

            if (IsMouseButtonReleased(MouseButton.Left)) btnAction = true;
        }
        else _btnState = 0;

        if (btnAction)
        {
            PlaySound(_fxButton);
            Thread.Sleep(50);
            return 1;
        }

        // Calculate button frame rectangle to draw depending on button state
        _sourceRec.Y = _btnState*_frameHeight;
        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------

        ClearBackground(Color.RayWhite);

        DrawTextureRec(_button, _sourceRec, new Vector2(_btnBounds.X, _btnBounds.Y), Color.White); // Draw button frame
        
        //----------------------------------------------------------------------------------
        return 0;
    }

    public override void InitScene()
    {
        InitAudioDevice();      // Initialize audio device

        _fxButton = LoadSound("Assets/UI-UX/Buttons/buttonfx.wav");   // Load button sound
        _button = LoadTexture("Assets/UI-UX/Buttons/button.png"); // Load button texture

        // Define frame rectangle for drawing
        _frameHeight = (float)_button.Height/NumFrames;
        _sourceRec = new Rectangle(0, 0, _button.Width, _frameHeight); //{ 0, 0, (float)button.Width, frameHeight };

        // Define button bounds on screen
        _btnBounds = new Rectangle(GetScreenWidth()/2.0f - _button.Width/2.0f, GetScreenHeight()/2.0f - (float)_button.Height/NumFrames/2.0f, _button.Width, _frameHeight ); //{ };

        _btnState = 0;               // Button state: 0-NORMAL, 1-MOUSE_HOVER, 2-PRESSED// Button action should be activated

        _mousePoint = new Vector2(0, 0);
    }

    public override void KillScene()
    {
        // De-Initialization
        //--------------------------------------------------------------------------------------
        UnloadTexture(_button);  // Unload button texture
        UnloadSound(_fxButton);  // Unload sound

        CloseAudioDevice();     // Close audio device
    }
}