using Assimp;
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
        public Color3 Color { get; set; }

        public Primitive(Vector3 pos, Color3 color)
        {
            Pos = pos;
            Color = color;
        }

        // Overridable function voor intersection (neemt aan dat de ray de richtings vector + p0 is)
        public abstract bool Intersect(Vector3 direction, Vector3 origin);
    }

    public class Sphere : Primitive
    { 

        // Radius of sphere
        public float Radius { get; set; }

        public Sphere(Vector3 pos, float radius, Color3 color) : base(pos, color)
        {
            Pos = pos;
            Radius = radius;
        }

        public Sphere(float x, float y, float z, float radius, float r, float g, float b) : this(
            new Vector3(x, y, z),
            radius,
            new Color3(r, g, b)
        )
        { }

        public override bool Intersect(Vector3 direction, Vector3 origin)
        {
            float a = Vector3.Dot(direction, direction);
            float b = 2 * Vector3.Dot(direction, origin - Pos);
            float c = Vector3.Dot(origin - Pos, origin - Pos) - Radius * Radius;
            float d = b * b - 4 * a * c;

            if (d < 0) return false;

            float t1 = (-b + (float)Math.Sqrt(d)) / (2 * a);
            float t2 = (-b - (float)Math.Sqrt(d)) / (2 * a);

            return t1 > 0 || t2 > 0;
        }
    }

    public class Plane : Primitive
    {
        // Normal vector of plane
        public Vector3 N { get; set; }

        public Plane(Vector3 n, Vector3 pos, Color3 color) : base(pos, color)
        {
            N = n;
            Pos = pos;
        }

        public Plane(float x, float y, float z, float ux, float uy, float uz, float r, float g, float b) : this(
            new Vector3(x, y, z),
            new Vector3(ux, uy, uz),
            new Color3(r, g, b)
        )
        { }

        public override bool Intersect(Vector3 ray, Vector3 origin)
        {
            throw new NotImplementedException();
        }
    }
}
