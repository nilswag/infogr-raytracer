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

        // Overridable function voor intersection
        public abstract Intersection Intersect(Vector3 direction, Vector3 origin);
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

        public override Intersection Intersect(Vector3 direction, Vector3 origin)
        {
            // Neemt aan dat de ray de !genormaliseerde! richtings vector + p0 is
            // Vul P(t) = direction * t + origin in bij ||P - M||^2 = r^2 (de sphere formule)
            // Dan los op in termen van t en krijg a, b en c
            float a = Vector3.Dot(direction, direction);
            float b = 2 * Vector3.Dot(direction, origin - Pos);
            float c = Vector3.Dot(origin - Pos, origin - Pos) - Radius * Radius;
            float d = b * b - 4 * a * c;

            // als discriminant kleiner is dan 0 dan raakt de ray de sphere niet.
            if (d < 0) return null;

            // vindt beide oplossingen
            float t1 = (-b + (float)Math.Sqrt(d)) / (2 * a);
            float t2 = (-b - (float)Math.Sqrt(d)) / (2 * a);

            // return de "zichtbare" intersection
            if (t1 < t2 && t1 > 0) {
                Vector3 position = direction * t1 + origin;
                Vector3 normal = position - this.Pos;
                return new Intersection(position, t1, this, Vector3.Normalize(normal));
            } else if (t2 <= t1 && t2 > 0)
            {
                Vector3 position = direction * t2 + origin;
                Vector3 normal = position - this.Pos;
                return new Intersection(position, t2, this, Vector3.Normalize(normal));
                
            } else
            {
                return null;
            }
        }
    }

    public class Plane : Primitive
    {
        // Normal vector of plane
        public Vector3 N { get; set; }

        public Plane(Vector3 n, Vector3 pos, Color3 color) : base(pos, color)
        {
            N = Vector3.Normalize(n);
            Pos = pos;
        }

        public Plane(float x, float y, float z, float ux, float uy, float uz, float r, float g, float b) : this(
            new Vector3(x, y, z),
            new Vector3(ux, uy, uz),
            new Color3(r, g, b)
        )
        { }

        public override Intersection Intersect(Vector3 direction, Vector3 origin)
        {
            // Neemt aan dat de ray de !genormaliseerde! richtings vector + p0 is
            // Vul P(t) = E + t*d in bij (P(t) - P0) * n^ = 0 en los op voor t
            float t = Vector3.Dot(this.Pos - origin, this.N)/Vector3.Dot(direction, this.N);

            if (t > 0) {
                Vector3 position = direction * t + origin;
                return new Intersection(position, t, this, this.N);
            } else
            {
                return null;
            }
           
        }
    }
}
