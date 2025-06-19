using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules.Camera;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World;

public class Water
{
    private readonly int _uTimeLoc;
    private readonly int _viewPosLoc;
    private readonly int _lightDirLoc;
    private readonly Shader _waterShader;
    private float  _timeAccumulator;
    private readonly Model _waterModel;
    
    private readonly Vector3 _position;

    public Water(Vector3 position, float width, float length)
    {
        _position  = position;
        // Load water shader
        _waterShader   = Raylib.LoadShader("Assets/Shaders/water.vert", "Assets/Shaders/water.frag");
        _uTimeLoc      = Raylib.GetShaderLocation(_waterShader, "uTime");
        _viewPosLoc    = Raylib.GetShaderLocation(_waterShader, "viewPos");
        _lightDirLoc   = Raylib.GetShaderLocation(_waterShader, "lightDir");

        _timeAccumulator = 0f;

        // Create a plane mesh with 100×100 subdivisions
        Mesh waterMesh = Raylib.GenMeshPlane(width, length, 100, 100);
        _waterModel = Raylib.LoadModelFromMesh(waterMesh);

        // Attach your water shader to the model's material
        unsafe
        {
            _waterModel.Materials[0].Shader = _waterShader;
        }
    }

    public void Update()
    {
        _timeAccumulator += Raylib.GetFrameTime();

        // Update shader uniforms
        Raylib.SetShaderValue(_waterShader, _uTimeLoc, new[] { _timeAccumulator }, ShaderUniformDataType.Float);
        // pass camera position
        Vector3 camPos = CharacterCamera3D.Camera.Position;
        Raylib.SetShaderValue(_waterShader, _viewPosLoc, new[] { camPos.X, camPos.Y, camPos.Z }, ShaderUniformDataType.Vec3);
        // simple directional light coming from above/front
        var light = Vector3.Normalize(new Vector3( 0.5f, -1.0f, 0.3f ));
        Raylib.SetShaderValue(_waterShader, _lightDirLoc, new[] { light.X, light.Y, light.Z }, ShaderUniformDataType.Vec3);
        
        Raylib.BeginBlendMode(BlendMode.Alpha);
        Rlgl.DisableDepthMask();  // don't write to depth

        // Update shader uniforms as before…
        _timeAccumulator += Raylib.GetFrameTime();
        Raylib.SetShaderValue(_waterShader, _uTimeLoc, new[] { _timeAccumulator }, ShaderUniformDataType.Float);
        camPos = CharacterCamera3D.Camera.Position;
        Raylib.SetShaderValue(_waterShader, _viewPosLoc, new[]{camPos.X,camPos.Y,camPos.Z}, ShaderUniformDataType.Vec3);
        light = Vector3.Normalize(new Vector3(0.5f, -1f, 0.3f));
        Raylib.SetShaderValue(_waterShader, _lightDirLoc, new[]{light.X,light.Y,light.Z}, ShaderUniformDataType.Vec3);

        // Draw the subdivided plane at y=0, centered on your cube’s footprint
        // (rotate so it faces up)
        Raylib.DrawModel(_waterModel,
            _position,    // position
            1f,                       // uniform scale
            Color.White               // color is ignored by shader
        );

        Rlgl.EnableDepthMask();
        Raylib.EndBlendMode();
    }

    public void Kill()
    {
        Raylib.UnloadShader(_waterShader);
    }
}