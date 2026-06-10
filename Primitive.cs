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
            // Neemt aan dat de ray de !genormaliseerde! richtings vector + p0 is
            // Vul P(t) = E + t*d in bij (P(t) - P0) * n^ = 0 en los op voor t
            float t = Vector3.Dot(this.Pos - origin, this.N)/Vector3.Dot(direction, this.N);

            if (t > 0) {
                Vector3 P = direction * t + origin;
                Vector3 normal = this.N;
                // adapt the normal
                if(Vector3.Dot(normal, direction) > 0) normal*=-1;

                //check whether P is inside triangle, otherwise no intersection
                if (Vector3.Dot(this.N, Vector3.Cross(B-A, P-A)) < 0) return null;
                if (Vector3.Dot(this.N, Vector3.Cross(C-B, P-B)) < 0) return null;
                if (Vector3.Dot(this.N, Vector3.Cross(A-C, P-C)) < 0) return null;

                float alfa = ((Vector3.Cross(C-B, P-B)).Length / 2)/((Vector3.Cross(C-B, A-B)).Length / 2);
                float beta = ((Vector3.Cross(A-C, P-C)).Length / 2)/((Vector3.Cross(C-B, A-B)).Length / 2);
                float gamma = 1 - alfa - beta;
                
                //normal interpolation
                Vector3 shadingNormal = Vector3.Normalize(alfa * NA + beta * NB + gamma * NC);
                if(Vector3.Dot(shadingNormal, direction) > 0) shadingNormal*=-1;
                //TODO: add texture coordinates also??

                return new Intersection(P, t, this, shadingNormal);
            } else
            {
                return null;
            }   
        }
    }
}