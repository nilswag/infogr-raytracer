using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Template
{
    public abstract class Primitive
    {
        // TODO: Ray intersection logic
        
        // Position of primitive
        public Vector3 Pos { get; set; }
        
        // Kleur van primitive
        public Vector3 Color { get; set; }

        public Primitive(Vector3 pos, Vector3 color)
        {
            Pos = pos;
            Color = color;
        }

        // Overridable function voor intersection (neemt aan dat de ray de richtings vector + p0 is)
        public abstract bool Intersect(Vector3 ray);
    }

    public class Sphere : Primitive
    { 

        // Radius of sphere
        public float Radius { get; set; }

        public Sphere(Vector3 pos, float radius, Vector3 color) : base(pos, color)
        {
            Pos = pos;
            Radius = radius;
        }

        public Sphere(float x, float y, float z, float radius, float r, float g, float b) : this(
            new Vector3(x, y, z),
            radius,
            new Vector3(r, g, b)
        )
        { }

        public override bool Intersect(Vector3 ray)
        {
            throw new NotImplementedException();
        }
    }

    public class Plane : Primitive
    {
        // Normal vector of plane
        public Vector3 N { get; set; }

        public Plane(Vector3 n, Vector3 pos, Vector3 color) : base(pos, color)
        {
            N = n;
            Pos = pos;
        }

        public Plane(float x, float y, float z, float ux, float uy, float uz, float r, float g, float b) : this(
            new Vector3(x, y, z),
            new Vector3(ux, uy, uz),
            new Vector3(r, g, b)
        )
        { }

        public override bool Intersect(Vector3 ray)
        {
            throw new NotImplementedException();
        }
    }
}
