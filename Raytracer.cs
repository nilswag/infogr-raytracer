using System;
using System.Collections.Generic;
using System.Text;

namespace Template
{
    public class Raytracer
    {
        public Camera Camera { get; set; }

        private RTScene Scene1;

        public Raytracer()
        {
            Scene1 = new RTScene(
                [], // array van lights, is voor nu leeg gezien we er niks mee doen
                [
                    //          x      y    z      r
                    new Sphere(-5.0f, 0.0f, 5.0f, 2.0f),
                    new Sphere(0.0f, 0.0f, 5.0f, 2.0f),
                    new Sphere(5.0f, 0.0f, 5.0f, 2.0f)
                ]
                );
        }

        public void Render()
        {

        }
    }
}
