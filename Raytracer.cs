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
        public Surface Surf { get; set; }

        // Camera
        public Camera Camera { get; set; }

        public Raytracer()
        {
            Surf = new Surface(640, 400); // groote is voor nu hard-coded op basis van template.cs
            Camera = new Camera(Vector3.Zero, Vector3.UnitZ);
        }

        public void Render(RTScene scene, Surface dest)
        {
            for (int x = 0; x < Surf.width; x++)
            {
                for (int y = 0; y < Surf.height; y++)
                {
                    // 1/2 van breedte/hoogte van image plane
                    float dx = (float)Math.Tan(Camera.FOV / 2.0 * (Math.PI / 180.0));
                    float dy = (float)(dx / Camera.AspectRatio);

                    // eerst normalizeren wij de pixels naar [0, 1] (daardoor wordt het een percentage)
                    float u = (float)x / Surf.width;
                    float v = (float)y / Surf.height;

                    // dan mappen we [0, 1] -> [-1, 1]
                    float px = 2 * u - 1.0f;
                    float py = 2 * v - 1.0f;

                    // nu maken wij de ray
                    // origin: camera positie
                    // richting (px * Camera.Right, py * Camera.Up)
                    // we scalen px ook met dx gezien we van [-1, 1] -> imagePlane willen mappen
                    Vector3 ray = Camera.Pos + Camera.Forward
                        + px * dx * Camera.Right
                        + py * dy * Camera.Up;

                    foreach (var obj in scene.Primitives)
                    {
                        if (!obj.Intersect(ray, Camera.Pos)) continue;
                        Surf.Plot(x, y, obj.Color);
                    }
                }
            }

            Surf.CopyTo(dest);
        }
    }
}
