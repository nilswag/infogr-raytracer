using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Template
{
    public class Light
    {
        // Position of point light
        public Vector3 Pos { get; set; }

        // Intensity of point light (I = (r, g, b))
        public Color3 Intensity { get; set; }

        // Direction of light (for spotlight)
        public Vector3 Direction { get; set; }

        // Cutoff of light (for spotlight)
        public float CutOff { get; set; } // set between 0 (lots of light) and 1 (tiny light)

        public Light(Vector3 pos, Color3 intensity)
        {
            Pos = pos;
            Intensity = intensity;
            Direction = new Vector3(0, 0, 0);
            CutOff = 0;
        }

        //spotlight
        public Light(Vector3 pos, Color3 intensity, Vector3 direction, float cutoff)
        {
            Pos = pos;
            Intensity = intensity;
            Direction = Vector3.Normalize(direction);
            CutOff = cutoff;
        }
    }
}
