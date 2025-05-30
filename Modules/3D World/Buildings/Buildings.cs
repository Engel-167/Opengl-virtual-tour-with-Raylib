using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World.Buildings;

public class Buildings(string path) : World3DObjects(path)
{
    //private const float CameraNearPlane = 0.01f; 
    //private const float CameraFarPlane = 1000.0f;
    public override void Draw3DModels()
    {
        // Get the camera's view OBB, passing the near and far planes
        /*Obb cameraViewObb = FrustumCulling.GetCameraViewObb(
            CharacterCamera3D.Camera, 
            (float)Raylib.GetScreenWidth() / Raylib.GetScreenHeight(),
            CameraNearPlane,
            CameraFarPlane);*/

        foreach (ModelData model in ModelDatas)
        {
            Raylib.DrawModelEx(model.Model, model.Position, model.Axis, model.Angle, model.Scale, Color.White);
            /*// Check if the model's OBB collides with the camera's view OBB
            if (Obb.CheckCollisionBoundingBoxVsObb(model.BoundingBox, cameraViewObb))
            {
                Raylib.DrawModelEx(model.Model, model.Position, model.Axis, model.Angle, model.Scale, Color.White);
            }*/
        }

    }

    public override void Unload3DModels()
    {
        foreach (ModelData model in ModelDatas)
        {
            Raylib.UnloadModel(model.Model);
        }
    }
}