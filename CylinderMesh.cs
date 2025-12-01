namespace Graphics
{
    internal class CylinderMesh : MeshComponent
    {
        private int segments = 32;
        private uint centerBottomIndex;
        private uint centerTopIndex;

        public CylinderMesh(Entity owner)
        {
            this.owner = owner;
            baseVertices = GetVertices();
            baseIndices = GetIndices();

            currentVertices = (float[])baseVertices.Clone();
            currentIndices = (uint[])baseIndices.Clone();
            setUp();
        }

        public override string Name => "Cylinder";

        protected override float[] GetVertices()
        {
            List<float> verts = new List<float>();

            // side
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / segments;
                float x = (float)Math.Cos(angle) * 0.5f;
                float z = (float)Math.Sin(angle) * 0.5f;
                float u = (float)i / segments;

                // normal front
                float nx = (float)Math.Cos(angle);
                float nz = (float)Math.Sin(angle);

                // bottom vertex
                verts.Add(x); verts.Add(0f); verts.Add(z);
                verts.Add(u); verts.Add(0f);
                verts.Add(nx); verts.Add(0f); verts.Add(nz);

                // top vertex
                verts.Add(x); verts.Add(1f); verts.Add(z);
                verts.Add(u); verts.Add(1f);
                verts.Add(nx); verts.Add(0f); verts.Add(nz);
            }

            // bottom circle
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / segments;
                float x = (float)Math.Cos(angle) * 0.5f;
                float z = (float)Math.Sin(angle) * 0.5f;
                float u = x + 0.5f;
                float v = z + 0.5f;

                verts.Add(x); verts.Add(0f); verts.Add(z);
                verts.Add(u); verts.Add(v);
                verts.Add(0f); verts.Add(-1f); verts.Add(0f); // normal down
            }
            // center bottom
            verts.Add(0f); verts.Add(0f); verts.Add(0f);
            verts.Add(0.5f); verts.Add(0.5f);
            verts.Add(0f); verts.Add(-1f); verts.Add(0f);
            centerBottomIndex = (uint)(verts.Count / 8 - 1);

            // top circle
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / segments;
                float x = (float)Math.Cos(angle) * 0.5f;
                float z = (float)Math.Sin(angle) * 0.5f;
                float u = x + 0.5f;
                float v = z + 0.5f;

                verts.Add(x); verts.Add(1f); verts.Add(z);
                verts.Add(u); verts.Add(v);
                verts.Add(0f); verts.Add(1f); verts.Add(0f); // normal op
            }
            // center top
            verts.Add(0f); verts.Add(1f); verts.Add(0f);
            verts.Add(0.5f); verts.Add(0.5f);
            verts.Add(0f); verts.Add(1f); verts.Add(0f);
            centerTopIndex = (uint)(verts.Count / 8 - 1);

            return verts.ToArray();
        }



        protected override uint[] GetIndices()
        {
            List<uint> inds = new List<uint>();

            uint sideVertsCount = (uint)((segments + 1) * 2);
            uint bottomCircleStart = sideVertsCount;
            uint centerBottom = centerBottomIndex;
            uint topCircleStart = bottomCircleStart + (uint)(segments + 1) + 1;
            uint centerTop = centerTopIndex;

            // Side
            for (uint i = 0; i < segments; i++)
            {
                uint bottomCurrent = i * 2;
                uint topCurrent = bottomCurrent + 1;
                uint bottomNext = (i + 1) * 2;
                uint topNext = bottomNext + 1;

                inds.Add(bottomCurrent);
                inds.Add(topCurrent);
                inds.Add(topNext);

                inds.Add(bottomCurrent);
                inds.Add(topNext);
                inds.Add(bottomNext);
            }

            // bottom
            for (uint i = 0; i < segments; i++)
            {
                uint current = bottomCircleStart + i;
                long next = bottomCircleStart + (i + 1) % segments;

                inds.Add(centerBottom);
                inds.Add(current);
                inds.Add((uint)next);
            }

            // Top
            for (uint i = 0; i < segments; i++)
            {
                uint current = topCircleStart + i;
                long next = topCircleStart + (i + 1) % segments;

                inds.Add(centerTop);
                inds.Add((uint)next);
                inds.Add(current);
            }

            return inds.ToArray();
        }
    }
}
