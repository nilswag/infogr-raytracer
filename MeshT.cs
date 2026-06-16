namespace Template
{
    public class MeshT
    {
        // List of triangles
        public List<Triangle> Triangles { get; set; }

        public Texture Texture { get; set; }

        public MeshT()
        {
            Triangles = new List<Triangle>();
        }
    }
}