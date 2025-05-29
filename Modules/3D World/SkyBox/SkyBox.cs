using System.Numerics;
using Raylib_cs;
namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World.SkyBox;
public class SkyBox
{
    public Model skyBox = new Model();
    public SkyBox()
    {
        Mesh Cube = Raylib.GenMeshCube(1.0f, 1.0f, 1.0f);
        skyBox = Raylib.LoadModelFromMesh(Cube);
        
        unsafe
        {
            skyBox.Materials[0].Shader = Raylib.LoadShader("Assets/Shaders/skybox.vert", "Assets/Shaders/skybox.frag");
            
            Raylib.SetShaderValue(skyBox.Materials[0].Shader, Raylib.GetShaderLocation(skyBox.Materials[0].Shader, "environmentMap"), (int)MaterialMapIndex.Cubemap, ShaderUniformDataType.Int);
            Raylib.SetShaderValue(skyBox.Materials[0].Shader, Raylib.GetShaderLocation(skyBox.Materials[0].Shader, "doGamma"), 0, ShaderUniformDataType.Int);
            Raylib.SetShaderValue(skyBox.Materials[0].Shader, Raylib.GetShaderLocation(skyBox.Materials[0].Shader, "vflipped"), 0, ShaderUniformDataType.Int);

            Image img = Raylib.LoadImage("Assets/SkyBox/pngwing.com.png"); //Raylib.LoadImage("Assets/SkyBox/space-skybox.png");
            skyBox.Materials[0].Maps[(int)MaterialMapIndex.Cubemap].Texture = Raylib.LoadTextureCubemap(img, CubemapLayout.AutoDetect);
            Raylib.UnloadImage(img);
            
        }
    }
    public unsafe void Destroy()
    {
        // Unload the cubemap texture
        Raylib.UnloadTexture(skyBox.Materials[0].Maps[(int)MaterialMapIndex.Cubemap].Texture);
        
        // Unload the shader
        Raylib.UnloadShader(skyBox.Materials[0].Shader);
        
        // Unload the model
        Raylib.UnloadModel(skyBox);
    }

    public void Draw()
    {
        Rlgl.DisableBackfaceCulling();
        Rlgl.DisableDepthMask();
        Raylib.DrawModel(skyBox, new Vector3(0, 0, 0), 1.0f, Color.White); // Changed scale to 1.0f
        Rlgl.EnableDepthMask();
        Rlgl.EnableBackfaceCulling();
    }

}