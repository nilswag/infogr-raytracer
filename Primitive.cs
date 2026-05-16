using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Template
{
    public class Primitive
    {
        // TODO: Ray intersection logic
        
        // Position of primitive
        public Vector3 Pos { get; set; }

        public Primitive(Vector3 pos)
        {
            Pos = pos;
        }
    }

    public class Sphere : Primitive
    { 

        // Radius of sphere
        public float Radius { get; set; }

        public Sphere(Vector3 pos, float radius) : base(pos)
        {
            Pos = pos;
            Radius = radius;
        }

        public Sphere(float x, float y, float z, float radius) : this(
            new Vector3(x, y, z),
            radius
        )
        { }
    }

    public class Plane : Primitive
    {
        // Normal vector of plane
        public Vector3 N { get; set; }

        public Plane(Vector3 n, Vector3 pos, Vector3 color) : base(pos)
        {
            N = n;
            Pos = pos;
        }

        public Plane(float x, float y, float z, float ux, float uy, float uz) : this(
            new Vector3(x, y, z),
            new Vector3(ux, uy, uz),
        )
        { }
    }
}
