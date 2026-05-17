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

        // Dictionary met alle scenes
        public Dictionary<string, RTScene> Scenes { get; }
        public string CurrentScene { get; set; }

        public Raytracer()
        {
            Display = new Surface(640, 400); // groote is voor nu hard-coded op basis van template.cs
            Camera = new Camera();
            Scenes = [];

            Scenes["basic"] = new RTScene(
                [], // array van lights, is voor nu leeg gezien we er niks mee doen
                [
                    //          x      y    z      r
                    new Sphere(-5.0f, 0.0f, 5.0f, 2.0f),
                    new Sphere(0.0f, 0.0f, 5.0f, 2.0f),
                    new Sphere(5.0f, 0.0f, 5.0f, 2.0f)
                ]
            );

            CurrentScene = "basic";
        }

        public void Render()
        {
            if (!Scenes.ContainsKey(CurrentScene)) return;

            Vector3 bl = Camera.Screen[0];
            Vector3 tr = Camera.Screen[2];

            for (float x = bl.X; x <= tr.X; x++)
            {
                for (float y = bl.Y; y <= tr.Y; y++)
                {
                    // TODO: Misschien dit beter abstracten
                    // Richtings vector van de ray (vanuit de camera)
                    Vector3 ray = new Vector3(x, y, bl.Z) - Camera.Pos;

                }
            }
        }
    }
}
