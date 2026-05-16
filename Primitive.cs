using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace INFOGRTemplate
{
    class Primitive
    {
        // TODO: Ray intersection logic
    }

    class Sphere : Primitive
    { 
        // Position of sphere
        public Vector3 Pos { get; set; }

        // Radius of sphere
        public float Radius { get; set; }

        public Sphere(Vector3 pos, float radius)
        {
            Pos = pos;
            Radius = radius;
        }
    }

    class Plane : Primitive
    {
        // Normal vector of plane
        public Vector3 N { get; set; }
    
        // Distance from origin to normal vector
        public Vector3 Pos { get; set; }

        public Plane(Vector3 n, Vector3 pos)
        {
            N = n;
            Pos = pos;
        }
    }
}
