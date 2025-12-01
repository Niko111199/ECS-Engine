using OpenTK.Graphics.OpenGL4;
using System.Numerics;

namespace Graphics
{
    internal class CustomeMeshComponent : MeshComponent
    {
        private string path;

        public CustomeMeshComponent(Entity owner)
        {
            this.owner = owner;
        }

        public void LoadModel(string path)
        {
            this.path = path;

            var loader = new OBJLoader(path);

            var verts = new List<float>();
            for (int i = 0; i < loader.Vertices.Length; i++)
            {
                var v = loader.Vertices[i];
                var uv = (i < loader.Uvs.Length) ? loader.Uvs[i] : Vector2.Zero;
                var n = (i < loader.Normals.Length) ? loader.Normals[i] : Vector3.UnitY;

                // position
                verts.Add(v.X);
                verts.Add(v.Y);
                verts.Add(v.Z);

                // uv
                verts.Add(uv.X);
                verts.Add(uv.Y);

                // normal
                verts.Add(n.X);
                verts.Add(n.Y);
                verts.Add(n.Z);
            }

            baseVertices = verts.ToArray();

            baseIndices = Enumerable.Range(0, loader.Vertices.Length)
                                    .Select(i => (uint)i)
                                    .ToArray();

            currentVertices = (float[])baseVertices.Clone();
            currentIndices = (uint[])baseIndices.Clone();

            setUp();
        }


        public override string Name => $"{Path.GetFileName(path)}";

        protected override float[] GetVertices() => baseVertices;
        protected override uint[] GetIndices() => baseIndices;
    }
}
