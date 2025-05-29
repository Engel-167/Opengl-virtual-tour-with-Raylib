namespace Opengl_virtual_tour_with_Raylib.Modules.Camera;
using System.Numerics;
using Raylib_cs; // Add this using directive for Raylib_cs.Camera3D and Raylib functions
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Hitboxes; // Add this for Obb

public enum BoundingBoxCornersFlag
{
    BOX_NO_CORNER = 0,

    BOX_FRONT_BOTTOM_LEFT = 1,
    BOX_FRONT_BOTTOM_RIGHT = 2,
    BOX_FRONT_TOP_LEFT = 4,
    BOX_FRONT_TOP_RIGHT = 8,

    BOX_BACK_BOTTOM_LEFT = 16,
    BOX_BACK_BOTTOM_RIGHT = 32,
    BOX_BACK_TOP_LEFT = 64,
    BOX_BACK_TOP_RIGHT = 128,

    BOX_ALL_CORNERS = 255
}

public struct Frustum
{
    public Vector4 Up;
    public Vector4 Down;
    public Vector4 Left;
    public Vector4 Right;
    public Vector4 Near;
    public Vector4 Far;
}

public static class FrustumCulling
{
    // Return the closest (orthogonal) signed distance between a point and a plane.
    // NOTE : A negative distance means the point is under the plane.
    public static float PlaneDistanceToPoint(Vector4 plane, Vector3 point)
    {
        // Plane equation: Ax + By + Cz + D = 0
        // plane.X = A, plane.Y = B, plane.Z = C, plane.W = D
        float d = point.X * plane.X + point.Y * plane.Y + point.Z * plane.Z + plane.W;
        float e = (float)Math.Sqrt(plane.X * plane.X + plane.Y * plane.Y + plane.Z * plane.Z);
        if (e == 0) return float.PositiveInfinity; // Avoid division by zero
        float distance = d / e;
        return distance;
    }

    // Check if the point is touching or is under the plane.
    public static bool CheckCollisionPlanePoint(Vector4 plane, Vector3 point)
    {
        return PlaneDistanceToPoint(plane, point) <= 0.0f;
    }

    // Check if the sphere is touching or is under the plane.
    public static bool CheckCollisionPlaneSphere(Vector4 plane, Vector3 center, float radius)
    {
        return PlaneDistanceToPoint(plane, center) <= radius;
    }

    public static bool CheckCollisionPlaneBox(Vector4 plane, BoundingBox box)
    {
        return CheckCollisionPlaneBoxEx(plane, box) != BoundingBoxCornersFlag.BOX_NO_CORNER;
    }

    public static BoundingBoxCornersFlag CheckCollisionPlaneBoxEx(Vector4 plane, BoundingBox box)
    {
        BoundingBoxCornersFlag corners = BoundingBoxCornersFlag.BOX_NO_CORNER;

        // Front face (min.Z)
        if (CheckCollisionPlanePoint(plane, new Vector3(box.Min.X, box.Min.Y, box.Min.Z))) corners |= BoundingBoxCornersFlag.BOX_FRONT_BOTTOM_LEFT;
        if (CheckCollisionPlanePoint(plane, new Vector3(box.Max.X, box.Min.Y, box.Min.Z))) corners |= BoundingBoxCornersFlag.BOX_FRONT_BOTTOM_RIGHT;
        if (CheckCollisionPlanePoint(plane, new Vector3(box.Min.X, box.Max.Y, box.Min.Z))) corners |= BoundingBoxCornersFlag.BOX_FRONT_TOP_LEFT;
        if (CheckCollisionPlanePoint(plane, new Vector3(box.Max.X, box.Max.Y, box.Min.Z))) corners |= BoundingBoxCornersFlag.BOX_FRONT_TOP_RIGHT;

        // Back face (max.Z)
        if (CheckCollisionPlanePoint(plane, new Vector3(box.Min.X, box.Min.Y, box.Max.Z))) corners |= BoundingBoxCornersFlag.BOX_BACK_BOTTOM_LEFT;
        if (CheckCollisionPlanePoint(plane, new Vector3(box.Max.X, box.Min.Y, box.Max.Z))) corners |= BoundingBoxCornersFlag.BOX_BACK_BOTTOM_RIGHT;
        if (CheckCollisionPlanePoint(plane, new Vector3(box.Min.X, box.Max.Y, box.Max.Z))) corners |= BoundingBoxCornersFlag.BOX_BACK_TOP_LEFT;
        if (CheckCollisionPlanePoint(plane, new Vector3(box.Max.X, box.Max.Y, box.Max.Z))) corners |= BoundingBoxCornersFlag.BOX_BACK_TOP_RIGHT;

        return corners;
    }

    // Return the frustum of the camera.
    // NOTE : The returned frustum is in World Space coordinates.
    // This now takes Raylib_cs.Camera3D directly.
    public static Frustum CameraGetFrustum(Raylib_cs.Camera3D camera, float aspect)
    {
        Frustum frustum = new Frustum();

        // Use Raylib's functions to get the view and projection matrices
        Matrix4x4 view = Raylib.GetCameraViewMatrix(ref camera);
        // Raylib's GetCameraProjectionMatrix also requires near and far planes.
        // For simplicity, we'll use default common values if not passed explicitly elsewhere.
        // In a real application, you'd pass these from where your camera is defined.
        float defaultNear = 0.01f; 
        float defaultFar = 1000.0f;
        Matrix4x4 proj = Raylib.GetCameraProjectionMatrix(ref camera, aspect); // Assumes fovy is set in camera

        // The frustum is calculated in World Space
        Matrix4x4 clip = view * proj;

        // Assuming perspective mode for now
        frustum.Left = Vector4.Normalize(new Vector4(clip.M14 + clip.M11, clip.M24 + clip.M21, clip.M34 + clip.M31, clip.M44 + clip.M41));
        frustum.Right = Vector4.Normalize(new Vector4(clip.M14 - clip.M11, clip.M24 - clip.M21, clip.M34 - clip.M31, clip.M44 - clip.M41));

        frustum.Down = Vector4.Normalize(new Vector4(clip.M14 + clip.M12, clip.M24 + clip.M22, clip.M34 + clip.M32, clip.M44 + clip.M42));
        frustum.Up = Vector4.Normalize(new Vector4(clip.M14 - clip.M12, clip.M24 - clip.M22, clip.M34 - clip.M32, clip.M44 - clip.M42));

        frustum.Near = Vector4.Normalize(new Vector4(clip.M14 + clip.M13, clip.M24 + clip.M23, clip.M34 + clip.M33, clip.M44 + clip.M43));
        frustum.Far = Vector4.Normalize(new Vector4(clip.M14 - clip.M13, clip.M24 - clip.M23, clip.M34 - clip.M33, clip.M44 - clip.M43));

        return frustum;
    }

    public static bool FrustumContainsSphere(Frustum frustum, Vector3 center, float radius)
    {
        if (PlaneDistanceToPoint(frustum.Left, center) < -radius) return false;
        if (PlaneDistanceToPoint(frustum.Right, center) < -radius) return false;
        if (PlaneDistanceToPoint(frustum.Up, center) < -radius) return false;
        if (PlaneDistanceToPoint(frustum.Down, center) < -radius) return false;
        if (PlaneDistanceToPoint(frustum.Far, center) < -radius) return false;
        if (PlaneDistanceToPoint(frustum.Near, center) < -radius) return false;
        return true;
    }

    public static bool FrustumContainsPoint(Frustum frustum, Vector3 point)
    {
        return FrustumContainsSphere(frustum, point, 0.0f);
    }

    public static bool FrustumContainsObb(Frustum frustum, Obb obb)
    {
        Vector3 planeNormal;
        
        Vector4[] planes = { frustum.Left, frustum.Right, frustum.Up, frustum.Down, frustum.Near, frustum.Far };

        foreach (Vector4 plane in planes)
        {
            planeNormal = new Vector3(plane.X, plane.Y, plane.Z);
            
            obb.GetAxes(out Vector3 right, out Vector3 up, out Vector3 forward);

            float obbRadius = 
                Math.Abs(Vector3.Dot(planeNormal, right) * obb.HalfExtents.X) +
                Math.Abs(Vector3.Dot(planeNormal, up) * obb.HalfExtents.Y) +
                Math.Abs(Vector3.Dot(planeNormal, forward) * obb.HalfExtents.Z);

            float centerDistance = PlaneDistanceToPoint(plane, obb.Center);

            if (centerDistance + obbRadius < 0)
            {
                return false; 
            }
        }
        
        return true;
    }
    
    // New method to get an OBB representing the camera's view volume
    public static Obb GetCameraViewObb(Camera3D camera, float aspect, float nearPlane, float farPlane)
    {
        // 1. Calculate the camera's forward direction
        // The camera's front vector is the direction from target to position
        Vector3 forward = Vector3.Normalize(camera.Target - camera.Position);

        // 2. Calculate the center of the OBB
        // This will be halfway between the near and far planes along the camera's view direction
        float viewDepth = farPlane - nearPlane;
        Vector3 obbCenter = camera.Position + forward * (nearPlane + viewDepth / 2.0f);

        // 3. Calculate the dimensions (half extents) of the OBB
        // These are based on the dimensions of the frustum at the far plane.
        // We'll calculate the width and height at the far plane.
        // Note: camera.fovy is in degrees, convert to radians.
        float tanFovY = (float)Math.Tan(camera.FovY * (Math.PI / 180.0f) * 0.5f); // Half FOV in radians
        float farHeight = 2.0f * tanFovY * farPlane;
        float farWidth = farHeight * aspect;

        // The half extents for the OBB will be half of the far plane dimensions.
        // The Z extent will be half the view depth (far - near).
        Vector3 obbHalfExtents = new Vector3(farWidth / 2.0f, farHeight / 2.0f, viewDepth / 2.0f);

        // 4. Calculate the OBB's rotation
        // This should match the camera's orientation.
        Matrix4x4 viewMatrix = Raylib.GetCameraViewMatrix(ref camera);
        
        Matrix4x4 worldTransform = Matrix4x4.Identity;
        Matrix4x4.Invert(viewMatrix, out worldTransform);
        Quaternion obbRotation = Quaternion.CreateFromRotationMatrix(worldTransform);

        return new Obb(obbCenter, obbHalfExtents, obbRotation);
    }
}