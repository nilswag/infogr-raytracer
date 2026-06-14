using OpenTK.Mathematics;


namespace Template
{
    public abstract class Primitive
    {
        // Positie van primitive
        public Vector3 Pos { get; set; }
        // Kleur van primitive
        public Color3 Color { get; set; }
        // Kleur van glossy primitive
        public Color3 SpecularColor { get; set; }
        // Specularity van glossy primitive
        public int Specularity { get; set; }
        // Kleur van mirror primitive
        public Color3 MirrorColor { get; set; }

        public Primitive(Vector3 pos, Color3 color)
        {
            Pos = pos;
            Color = color;
            SpecularColor = new Color3(0, 0, 0);;
            Specularity = 1;
            MirrorColor = new Color3(0, 0, 0);
        }

        public Primitive(Vector3 pos, Color3 color, Color3 specularColor, int specularity, Color3 mirrorColor)
        {
            Pos = pos;
            Color = color;
            SpecularColor = specularColor;
            Specularity = specularity;
            MirrorColor = mirrorColor;
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
            Radius = radius;
        }

        public Sphere(Vector3 pos, float radius, Color3 color, Color3 specularColor, int specularity, Color3 mirrorColor) : base(pos, color, specularColor, specularity, mirrorColor)
        {
            Radius = radius;
        }

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

            float t = float.PositiveInfinity;

            if(t1 > 0) t=t1;
            if(t2 > 0 && t2 < t) t = t2;
            if(t == float.PositiveInfinity) return null;
        
            Vector3 position = direction * t + origin;
            Vector3 normal = position - this.Pos;
            // adapt the normal
            if(Vector3.Dot(normal, direction) > 0) normal*=-1;
            return new Intersection(position, t, this, Vector3.Normalize(normal));
        }
    }

    public class Plane : Primitive
    {
        // Normal vector of plane
        public Vector3 N { get; set; }

        public Plane(Vector3 n, Vector3 pos, Color3 color) : base(pos, color)
        {
            N = Vector3.Normalize(n);
        }

        public Plane(Vector3 n, Vector3 pos, Color3 color, Color3 specularColor, int specularity, Color3 mirrorColor) : base(pos, color, specularColor, specularity, mirrorColor)
        {
            N = Vector3.Normalize(n);
        }

        public override Intersection Intersect(Vector3 direction, Vector3 origin)
        {
            // Neemt aan dat de ray de !genormaliseerde! richtings vector + p0 is
            // Vul P(t) = E + t*d in bij (P(t) - P0) * n^ = 0 en los op voor t
            float t = Vector3.Dot(this.Pos - origin, this.N)/Vector3.Dot(direction, this.N);

            if (t > 0) {
                Vector3 position = direction * t + origin;
                Vector3 normal = this.N;
                // adapt the normal
                if(Vector3.Dot(normal, direction) > 0) normal*=-1;
                return new Intersection(position, t, this, normal);
            } else
            {
                return null;
            }
           
        }
    }
    
    public class Triangle : Primitive
    {
        // Vertex positions
        public Vector3 A { get; set; }
        public Vector3 B { get; set; }
        public Vector3 C { get; set; }

        //Geometric normal
        public Vector3 N { get; set; }

        //Vertex normals
        public Vector3 NA { get; set; }
        public Vector3 NB { get; set; }
        public Vector3 NC { get; set; }

        //Texture coordinates
        public Vector2 UVA { get; set; }
        public Vector2 UVB { get; set; }
        public Vector2 UVC { get; set; }

        public Triangle(Vector3 a, Vector3 b, Vector3 c, Color3 color): base(a, color)
        {
            A = a;
            B = b;
            C = c;
            N = Vector3.Normalize(Vector3.Cross(B-A, C-A));
            NA = N;
            NB = N;
            NC = N;
        }

        public Triangle(Vector3 a, Vector3 b, Vector3 c, Color3 color, Color3 specularColor, int specularity, Color3 mirrorColor): base(a, color, specularColor, specularity, mirrorColor)
        {
            A = a;
            B = b;
            C = c;
            N = Vector3.Normalize(Vector3.Cross(B-A, C-A));
            NA = N;
            NB = N;
            NC = N;
        }
        public Triangle(Vector3 a, Vector3 b, Vector3 c, Vector3 na, Vector3 nb, Vector3 nc, Vector2 uva, Vector2 uvb, Vector2 uvc, Color3 color): base(a, color)
        {
            A = a;
            B = b;
            C = c;
            N = Vector3.Normalize(Vector3.Cross(B-A, C-A));
            NA = na;
            NB = nb;
            NC = nc;
            UVA = uva;
            UVB = uvb;
            UVC = uvc;
        }

        public override Intersection Intersect(Vector3 direction, Vector3 origin)
        {
            float dDotN = Vector3.Dot(direction, N);

            if (MathF.Abs(dDotN) < 1e-8f) return null;

            // Neemt aan dat de ray de !genormaliseerde! richtings vector + p0 is
            // Vul P(t) = E + t*d in bij (P(t) - P0) * n^ = 0 en los op voor t
            float t = Vector3.Dot(Pos - origin, N) / dDotN;
            if (t <= 0f) return null;

            Vector3 P = origin + direction * t;

            Vector3 AB = B - A;
            Vector3 BC = C - B;
            Vector3 CA = A - C;
            Vector3 AP = P - A;
            Vector3 BP = P - B;
            Vector3 CP = P - C;

            Vector3 crossAB_AP = Vector3.Cross(AB, AP);
            Vector3 crossBC_BP = Vector3.Cross(BC, BP);
            Vector3 crossCA_CP = Vector3.Cross(CA, CP);

            // half of the magnitude of the cross product of two vectors is equal to the area of the sub-triangle
            float d0 = Vector3.Dot(N, crossAB_AP); // signed area of parallelogram formed by AB and AP
            float d1 = Vector3.Dot(N, crossBC_BP); // signed area of parallelogram formed by BC and BP
            float d2 = Vector3.Dot(N, crossCA_CP); // signed area of parallelogram formed by CA and CP
            // this also automatically flips the normal correctly

            //check whether P is inside triangle, otherwise no intersection
            if (d0 < 0f || d1 < 0f || d2 < 0f) return null;

            // there is also no need to calculate the explicit area of the sub triangles for the barycentric coordinates
            // since the ratio of the cross products remains the same
            float denom = d0 + d1 + d2; // <---- area of parallelogram formed by any two edges connected by same vertex

            float alfa = d1 / denom;
            float beta = d2 / denom;
            float gamma = d0 / denom;

            //normal interpolation
            Vector3 shadingNormal = Vector3.Normalize(alfa * NA + beta * NB + gamma * NC);
            if(Vector3.Dot(shadingNormal, direction) > 0) shadingNormal*=-1;
            //TODO: add texture coordinates also??

            return new Intersection(P, t, this, shadingNormal); 
        }
    }
}