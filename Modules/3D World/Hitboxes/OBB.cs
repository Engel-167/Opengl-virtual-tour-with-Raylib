namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World.Hitboxes;

using Raylib_cs;
using System;
using System.Numerics;


public struct Obb
{
    public Quaternion Rotation;
    public Vector3 Center;
    public Vector3 HalfExtents;

    public Obb(Vector3 center, Vector3 halfExtents, Quaternion rotation)
    {
        Center = center;
        HalfExtents = halfExtents;
        Rotation = rotation;
    }

    //Local Axis rotated
    public void GetAxes(out Vector3 right, out Vector3 up, out Vector3 forward)
    {
        Matrix4x4 rot = Matrix4x4.CreateFromQuaternion(Rotation);

        right = new Vector3(rot.M11, rot.M12, rot.M13);

        up = new Vector3(rot.M21, rot.M22, rot.M23);

        forward = new Vector3(rot.M31, rot.M32, rot.M33);
    }

    public Vector3[] GetCorners()
    {
        GetAxes(out Vector3 right, out Vector3 up, out Vector3 forward);
        right *= HalfExtents.X;
        up *= HalfExtents.Y;
        forward *= HalfExtents.Z;

        Vector3[] c = new Vector3[8];

        c[0] = Center + right + up + forward;
        c[1] = Center - right + up + forward;
        c[2] = Center - right + up - forward;
        c[3] = Center + right + up - forward;

        c[4] = Center + right - up + forward;
        c[5] = Center - right - up + forward;
        c[6] = Center - right - up - forward;
        c[7] = Center + right - up - forward;

        return c;
    }

    public void DrawWireFrame(Color color)
    {
        Vector3[] c = GetCorners();

        Raylib.DrawLine3D(c[0], c[1], color);
        Raylib.DrawLine3D(c[1], c[2], color);
        Raylib.DrawLine3D(c[2], c[3], color);
        Raylib.DrawLine3D(c[3], c[0], color);

        Raylib.DrawLine3D(c[4], c[5], color);
        Raylib.DrawLine3D(c[5], c[6], color);
        Raylib.DrawLine3D(c[6], c[7], color);
        Raylib.DrawLine3D(c[7], c[4], color);

        Raylib.DrawLine3D(c[0], c[4], color);
        Raylib.DrawLine3D(c[1], c[5], color);
        Raylib.DrawLine3D(c[2], c[6], color);
        Raylib.DrawLine3D(c[3], c[7], color);
    }

    public bool ContainsPoint(Vector3 point)
    {
        Vector3 local = point - Center;
        Quaternion invRot = Quaternion.Inverse(Rotation);
        local = Vector3.Transform(local, invRot);

        return Math.Abs(local.X) <= HalfExtents.X &&
               Math.Abs(local.Y) <= HalfExtents.Y &&
               Math.Abs(local.Z) <= HalfExtents.Z;

    }

    public static void ProjectBoundingBoxOntoAxis(BoundingBox box, Vector3 axis, out float min, out float max)
    {
        Vector3[] corners =
        [
            new Vector3(box.Min.X, box.Min.Y, box.Min.Z),
            new Vector3(box.Max.X, box.Min.Y, box.Min.Z),
            new Vector3(box.Max.X, box.Max.Y, box.Min.Z),
            new Vector3(box.Min.X, box.Max.Y, box.Min.Z),
            new Vector3(box.Min.X, box.Min.Y, box.Max.Z),
            new Vector3(box.Max.X, box.Min.Y, box.Max.Z),
            new Vector3(box.Max.X, box.Max.Y, box.Max.Z),
            new Vector3(box.Min.X, box.Max.Y, box.Max.Z)
        ];

        min = Vector3.Dot(corners[0], axis);
        max = min;

        for (int i = 1; i < 8; ++i)
        {
            float projection = Vector3.Dot(corners[i], axis);
            if (projection < min) min = projection;
            if (projection > max) max = projection;
        }
    }

    public void ProjectOntoAxis(Vector3 axis, out float min, out float max)
    {
        GetAxes(out Vector3 right, out Vector3 up, out Vector3 forward);

        float r =
            Math.Abs(Vector3.Dot(right, axis)) * HalfExtents.X +
            Math.Abs(Vector3.Dot(up, axis)) * HalfExtents.Y +
            Math.Abs(Vector3.Dot(forward, axis)) * HalfExtents.Z;

        float centerProj = Vector3.Dot(Center, axis);
        min = centerProj - r;
        max = centerProj + r;
    }

    public static bool CheckCollisionBoundingBoxVsObb(BoundingBox box, Obb obb)
    {
        Vector3[] aabbAxes =
        {
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, 1)
        };

        obb.GetAxes(out Vector3 obbRight, out Vector3 obbUp, out Vector3 obbForward);
        Vector3[] obbAxes = { obbRight, obbUp, obbForward };

        Vector3[] testAxes = new Vector3[15];
        int axisCount = 0;

        for (int i = 0; i < 3; i++) testAxes[axisCount++] = aabbAxes[i];
        for (int i = 0; i < 3; i++) testAxes[axisCount++] = obbAxes[i];
        for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
        {
            Vector3 cross = Vector3.Cross(aabbAxes[i], obbAxes[j]);
            if (cross.LengthSquared() > 0.000001f)
                testAxes[axisCount++] = Vector3.Normalize(cross);
        }

        for (int i = 0; i < axisCount; ++i)
        {
            Vector3 axis = testAxes[i];
            ProjectBoundingBoxOntoAxis(box, axis, out float minA, out float maxA);
            obb.ProjectOntoAxis(axis, out float minB, out float maxB);

            if (maxA < minB || maxB < minA)
                return false;
        }

        return true;
    }

    public static bool CheckCollisionObbvsObb(Obb a, Obb b)
    {
        Vector3[] axesA = new Vector3[3];
        Vector3[] axesB = new Vector3[3];
        a.GetAxes(out axesA[0], out axesA[1], out axesA[2]);
        b.GetAxes(out axesB[0], out axesB[1], out axesB[2]);

        Vector3[] testAxes = new Vector3[15];
        int axisCount = 0;

        // 3 ejes de A
        for (int i = 0; i < 3; ++i) testAxes[axisCount++] = axesA[i];
        // 3 ejes de B
        for (int i = 0; i < 3; ++i) testAxes[axisCount++] = axesB[i];
        // 9 cross-products (ejes potencialmente únicos)
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                Vector3 cross = Vector3.Cross(axesA[i], axesB[j]);
                float len = cross.Length();
                if (len > 1e-6f) // No incluir ejes nulos
                    testAxes[axisCount++] = cross / len;
            }
        }

        for (int i = 0; i < axisCount; ++i)
        {
            Vector3 axis = testAxes[i];
            a.ProjectOntoAxis(axis, out float minA, out float maxA);
            b.ProjectOntoAxis(axis, out float minB, out float maxB);

            // Si no hay solapamiento, NO hay colisión
            if (maxA < minB || maxB < minA)
                return false;
        }

        // Si pasan todos los ejes: colisión
        return true;
    }
    
     public RayCollision GetRayCollision(Ray ray)
    {
        RayCollision result = new RayCollision
        {
            Hit = false,
            Distance = 0,
            Normal = new Vector3(0.0f, 0.0f, 0.0f),
            Point = new Vector3(0.0f, 0.0f, 0.0f)
        };

        // Mover el rayo al espacio local de la OBB
        Vector3 localOrigin = ray.Position - Center;
        Quaternion inverseRot = Quaternion.Inverse(Rotation);
        Vector3 localRayOrigin = Vector3.Transform(localOrigin, inverseRot);
        Vector3 localRayDir = Vector3.Transform(ray.Direction, inverseRot);

        Vector3 boxMin = -HalfExtents;
        Vector3 boxMax = HalfExtents;

        float tmin = float.NegativeInfinity;
        float tmax = float.PositiveInfinity;
        Vector3 normal = Vector3.Zero;

        for (int i = 0; i < 3; ++i)
        {
            float origin = i == 0 ? localRayOrigin.X : (i == 1 ? localRayOrigin.Y : localRayOrigin.Z);
            float dir = i == 0 ? localRayDir.X : (i == 1 ? localRayDir.Y : localRayDir.Z);
            float min = i == 0 ? boxMin.X : (i == 1 ? boxMin.Y : boxMin.Z);
            float max = i == 0 ? boxMax.X : (i == 1 ? boxMax.Y : boxMax.Z);

            if (Math.Abs(dir) < 0.0001f)
            {
                if (origin < min || origin > max)
                    return result; // No colisión
            }
            else
            {
                float ood = 1.0f / dir;
                float t1 = (min - origin) * ood;
                float t2 = (max - origin) * ood;
                int axis = i;

                if (t1 > t2)
                {
                    (t1, t2) = (t2, t1);
                    axis = -axis;
                }

                if (t1 > tmin)
                {
                    tmin = t1;
                    normal = Vector3.Zero;
                    int absAxis = Math.Abs(axis);
                    float sign = axis >= 0 ? -1.0f : 1.0f;

                    if (absAxis == 0) normal.X = sign;
                    else if (absAxis == 1) normal.Y = sign;
                    else normal.Z = sign;
                }

                if (t2 < tmax)
                    tmax = t2;

                if (tmin > tmax)
                    return result; // No colisión
            }
        }

        result.Hit = true;
        result.Distance = tmin;
        result.Point = ray.Position + ray.Direction * tmin;
        // Normal transformada al espacio global
        result.Normal = Vector3.TransformNormal(normal, Matrix4x4.CreateFromQuaternion(Rotation));

        return result;
    }
        
     // Esfera vs OBB
    public bool CheckCollisionSphere(Vector3 sphereCenter, float radius) 
    {
        Vector3 localCenter = sphereCenter - Center; 
        Quaternion invRot = Quaternion.Inverse(Rotation); 
        localCenter = Vector3.Transform(localCenter, invRot);
        
        Vector3 clamped = new Vector3(
            Math.Clamp(localCenter.X, -HalfExtents.X, HalfExtents.X),
            Math.Clamp(localCenter.Y, -HalfExtents.Y, HalfExtents.Y),
            Math.Clamp(localCenter.Z, -HalfExtents.Z, HalfExtents.Z)
            );

        Vector3 worldClamped = Vector3.Transform(clamped, Rotation) + Center;
        float distSq = Vector3.DistanceSquared(sphereCenter, worldClamped);
        return distSq <= radius * radius;
    }
}
