using Assimp;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Template
{
    public class Raytracer
    {
        // Surface waar de raytracer op gaat werken
        public Surface Display { get; set; }

        // Camera
        public Camera Camera { get; set; }

        public Raytracer()
        {
            Display = new Surface(640, 400); // groote is voor nu hard-coded op basis van template.cs
            Camera = new Camera();
        }

        public void Render(RTScene scene, Surface dest)
        {
           
        }
    }
}
