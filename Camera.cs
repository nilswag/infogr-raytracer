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

        public Camera(Vector3 pos, Vector3 target, float fov, float aspectRatio)
        {
            Pos = pos;
            FOV = fov;
            AspectRatio = aspectRatio;
            LookAt(target);
            ImagePlane = new Vector3[4]; 
            SetPoints();
        }


        public void LookAt(Vector3 target)
        {
            Forward = Vector3.Normalize(target - Pos);
            Right = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, Forward));
            Up = Vector3.Normalize(Vector3.Cross(Forward, Right));
        }

        public void SetPoints()
        {
            // Dit is onder aanname dat afstand van de camera tot plane center = 1. Die stellen we dus als vast, waardoor FOV
            // makkelijk veranderbaar blijft.
            
            // 1/2 van breedte/hoogte van image plane:
            float dx = (float)Math.Tan(FOV / 2.0 * (Math.PI / 180.0));
            float dy = (float)(dx / AspectRatio);

            // Center
            Vector3 c = Pos + 1 * Forward; //want d = 1

            // Hoekpunten
            ImagePlane[0] = c + dy * Up - dx * Right; //topleft
            ImagePlane[1] = c + dy * Up + dx * Right; //topright
            ImagePlane[2] = c - dy * Up - dx * Right; //bottomleft
            ImagePlane[3] = c - dy * Up + dx * Right; //bottom-right

        }
    }
}
