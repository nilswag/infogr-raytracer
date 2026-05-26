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

        public Light(Vector3 pos, Color3 intensity)
        {
            Pos = pos;
            Intensity = intensity;
        }

        public Light(float x, float y, float z, float r, float g, float b) : this(
            new Vector3(x, y, z),
            new Color3(r, g, b)
        )
        { }
    }
}
