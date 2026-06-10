using Assimp;
using OpenTK.Mathematics;

namespace Template
{
    public static class Util
    {
        // conversions from Assimp datatypes to corresponding OpenTK datatypes
        public static Matrix4 ToOpenTK(this Matrix4x4 m)
        {
            return new Matrix4(m.A1, m.A2, m.A3, m.A4,
                m.B1, m.B2, m.B3, m.B4,
                m.C1, m.C2, m.C3, m.C4,
                m.D1, m.D2, m.D3, m.D4);
        }
        public static Vector3 ToOpenTK(this Vector3D v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        // import any mesh file format supported by Assimp
        // currently supports only vertex positions, vertex normals, and texture coordinates
        // currently merges all sub-meshes in the file into one big result mesh
        // currently doesn't support materials, separate sub-meshes, or animations
        // TODO: insert your code to finish converting Assimp's internal data to your own mesh class
        public static MeshT ImportMesh(string filename)
        {
            MeshT result = new MeshT();
            AssimpContext importer = new();
            Scene model = importer.ImportFile(filename, PostProcessSteps.Triangulate);
            Node node = model.RootNode;
            Matrix4 prev = Matrix4.Identity;
            ImportMesh(model, node, prev, result);

            return result;
        }
        private static void ImportMesh(Scene model, Node node, Matrix4 prev, MeshT result)
        {
            Matrix4 transform = Matrix4.Mult(prev, node.Transform.ToOpenTK());
            Matrix3 normalTransform = new Matrix3(transform).Inverted().Transposed();
            if (node.HasMeshes)
            {
                foreach (int index in node.MeshIndices)
                {
                    Mesh mesh = model.Meshes[index];
                    bool hasTexCoords = mesh.HasTextureCoords(0);
                    foreach (Face face in mesh.Faces)
                    {
                        Vector3 p1 = (transform * new Vector4(mesh.Vertices[face.Indices[0]].ToOpenTK(), 1)).Xyz;
                        Vector3 p2 = (transform * new Vector4(mesh.Vertices[face.Indices[1]].ToOpenTK(), 1)).Xyz;
                        Vector3 p3 = (transform * new Vector4(mesh.Vertices[face.Indices[2]].ToOpenTK(), 1)).Xyz;
                        Triangle tri = new Triangle(p1, p2, p3, new Color3(0.5f, 0.5f, 0.5f)); //add color choice?
                        if (mesh.HasNormals && hasTexCoords)
                        {
                            tri.NA = normalTransform * mesh.Normals[face.Indices[0]].ToOpenTK();
                            tri.NB = normalTransform * mesh.Normals[face.Indices[1]].ToOpenTK();
                            tri.NC = normalTransform * mesh.Normals[face.Indices[2]].ToOpenTK();
                        }
                        if (hasTexCoords)
                        {
                            Vector2 offset = new(0, 1);
                            Vector2 scale = new(1, -1);
                            tri.UVA = mesh.TextureCoordinateChannels[0][face.Indices[0]].ToOpenTK().Xy * scale + offset;
                            tri.UVB = mesh.TextureCoordinateChannels[0][face.Indices[1]].ToOpenTK().Xy * scale + offset;
                            tri.UVC = mesh.TextureCoordinateChannels[0][face.Indices[2]].ToOpenTK().Xy * scale + offset;
                            
                        }
                        result.Triangles.Add(tri);
                    }
                }
            }
            for (int i = 0; i < node.ChildCount; i++)
            {
                ImportMesh(model, node.Children[i], transform, result);
            }
        }
    }
}
