using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Template
{
    public class Camera
    {
        // Positie van camera
        public Vector3 Pos { get; set; }

        // Richting van camera
        public Vector3 Forward { get; set; }
        public Vector3 Right { get; set; }
        public Vector3 Up { get; set; }

        // FOV in graden
        public float FOV { get; set; }

        // Aspect ratio
        public float AspectRatio { get; set; }

        public Vector3[] ImagePlane { get; }

        public Camera(Vector3 pos, float fov, float aspectRatio, Vector3 target)
        {
            Pos = pos;
            FOV = fov;
            AspectRatio = aspectRatio;
            ImagePlane = new Vector3[2];
            LookAt(target);
        }

        public void LookAt(Vector3 target)
        {
            Forward = Vector3.Normalize(target - Pos);
            Right = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, Forward));
            Up = Vector3.Normalize(Vector3.Cross(Forward, Right));


        }
    }
}
