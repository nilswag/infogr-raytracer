using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Template;

public class Camera
{
    public Vector3 Pos { get; set; }

    public Vector3 Forward { get; private set; }
    public Vector3 Right { get; private set; }
    public Vector3 Up { get; private set; }

    public float FOV { get; set; }
    public float AspectRatio { get; set; }

    public float Yaw { get; private set; }
    public float Pitch { get; private set; }

    public Vector3[] ImagePlane { get; }

    public Camera(Vector3 pos, Vector3 forward, float fov, float aspectRatio)
    {
        Pos = pos;
        FOV = fov;
        AspectRatio = aspectRatio;
        ImagePlane = new Vector3[4];

        Forward = Vector3.Normalize(forward);
        Yaw = MathHelper.RadiansToDegrees(MathF.Atan2(Forward.Z, Forward.X));
        Pitch = MathHelper.RadiansToDegrees(MathF.Asin(Forward.Y));

        UpdateVectors();
        SetPoints();
    }

    public void Rotate(float yawDelta, float pitchDelta)
    {
        Yaw += yawDelta;
        Pitch += pitchDelta;
        Pitch = Math.Clamp(Pitch, -89f, 89f);

        UpdateVectors();
        SetPoints();
    }

    private void UpdateVectors()
    {
        float yawRad = MathHelper.DegreesToRadians(Yaw);
        float pitchRad = MathHelper.DegreesToRadians(Pitch);

        Vector3 forward;
        forward.X = MathF.Cos(yawRad) * MathF.Cos(pitchRad);
        forward.Y = MathF.Sin(pitchRad);
        forward.Z = MathF.Sin(yawRad) * MathF.Cos(pitchRad);

        Forward = Vector3.Normalize(forward);
        Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(Right, Forward));
    }

    public void SetPoints()
    {
        float dx = (float)Math.Tan(FOV / 2.0 * (Math.PI / 180.0));
        float dy = dx / AspectRatio;

        Vector3 c = Pos + Forward;

        ImagePlane[0] = c + dy * Up - dx * Right;
        ImagePlane[1] = c + dy * Up + dx * Right;
        ImagePlane[2] = c - dy * Up - dx * Right;
        ImagePlane[3] = c - dy * Up + dx * Right;
    }
}