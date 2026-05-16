using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace INFOGRTemplate
{
    public class Light
    {
        // Position of point light
        public Vector3 Pos { get; set; }

        // Intensity of point light (I = (r, g, b))
        public Vector3 Intensity { get; set; }

        public Light(Vector3 pos, Vector3 intensity)
        {
            Pos = pos;
            Intensity = intensity;
        }
    }
}
