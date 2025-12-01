
namespace Graphics
{
    internal class SphereMesh : MeshComponent
    {
        public SphereMesh(Entity owner)
        {
            this.owner = owner;
            baseVertices = GetVertices();
            baseIndices = GetIndices();

            currentVertices = (float[])baseVertices.Clone();
            currentIndices = (uint[])baseIndices.Clone();
            setUp();
        }

        private int rings = 16;
        private int sectors = 32;
        public override string Name => "Sphere";

        protected override float[] GetVertices()
        {
            List<float> verts = new List<float>();

            for (int r = 0; r <= rings; r++)
            {
                float theta = r * (float)Math.PI / rings;
                float sinTheta = (float)Math.Sin(theta);
                float cosTheta = (float)Math.Cos(theta);

                for (int s = 0; s <= sectors; s++)
                {
                    float phi = s * 2 * (float)Math.PI / sectors;
                    float sinPhi = (float)Math.Sin(phi);
                    float cosPhi = (float)Math.Cos(phi);

                    float nx = cosPhi * sinTheta;
                    float ny = cosTheta;
                    float nz = sinPhi * sinTheta;

                    float x = nx * 0.5f;
                    float y = ny * 0.5f;
                    float z = nz * 0.5f;

                    float u = s / (float)sectors;
                    float v = 1.0f - r / (float)rings;

                    verts.Add(x); verts.Add(y); verts.Add(z);       // position
                    verts.Add(u); verts.Add(v);                     // uv
                    verts.Add(nx); verts.Add(ny); verts.Add(nz);    //normal
                }
            }

            return verts.ToArray();
        }

        protected override uint[] GetIndices()
        {
            List<uint> inds = new List<uint>();

            for (uint r = 0; r < rings; r++)
            {
                for (uint s = 0; s < sectors; s++)
                {
                    uint first = r * (uint)(sectors + 1) + s;
                    uint second = first + (uint)(sectors + 1);

                    inds.Add(first);
                    inds.Add(first + 1);
                    inds.Add(second);

                    inds.Add(second);
                    inds.Add(first + 1);
                    inds.Add(second + 1);
                }
            }
            return inds.ToArray();
        }
    }
}
