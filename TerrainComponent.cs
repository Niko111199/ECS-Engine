using OpenTK.Mathematics;
using System;
using System.IO;
using System.Collections.Generic;

namespace Graphics
{
    internal class TerrainComponent : MeshComponent
    {
        public override string Name => "terrain";

        private int width, height;
        private float heightIntensity = 1.0f;
        private float[] heightData;
        private int size = 2;
        private Entity owner;

        public Vector3 scale = Vector3.One;

        public TerrainComponent(Entity owner)
        {
            this.owner = owner;
            baseVertices = GetVertices();
            baseIndices = GetIndices();

            currentVertices = (float[])baseVertices.Clone();
            currentIndices = (uint[])baseIndices.Clone();
            setUp();
        }

        protected override float[] GetVertices()
        {
            return GenerateFlatPlane(size, size);
        }

        protected override uint[] GetIndices()
        {
            return GeneratePlaneIndices(size, size);
        }

        public void ApplyHeightMap(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"[ERROR] Heightmap not found: {path}");
                return;
            }

            Console.WriteLine($"Loading heightmap: {path}");

            byte[] bytes = File.ReadAllBytes(path);
            int width = bytes[12] | (bytes[13] << 8);
            int height = bytes[14] | (bytes[15] << 8);
            int pixelBits = bytes[16];

            this.width = width;
            this.height = height;

            heightData = new float[width * height];

            int pixelStart = 18 + bytes[0];
            int colorChannels = pixelBits / 8;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int i = (y * width + x) * colorChannels + pixelStart;
                    byte gray = bytes[i];
                    heightData[y * width + x] = gray / 255.0f;
                }
            }

            UpdateMesh();
        }

        public void SetHeightIntensity(float intensity)
        {
            heightIntensity = intensity;
            if (heightData != null)
                UpdateMesh();
        }

        private void UpdateMesh()
        {
            if (heightData == null) return;

            int w = width;
            int h = height;

            var vertices = new float[w * h * 8];
            int index = 0;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float xpos = (x / (float)(w - 1)) - 0.5f;
                    float ypos = heightData[y * w + x] * heightIntensity;
                    float zpos = (y / (float)(h - 1)) - 0.5f;

                    // Position
                    vertices[index++] = xpos;
                    vertices[index++] = ypos;
                    vertices[index++] = zpos;

                    // Texcoord
                    vertices[index++] = x / (float)(w - 1);
                    vertices[index++] = y / (float)(h - 1);

                    // Normal (midlertidig opad – kan beregnes fra heightmap)
                    vertices[index++] = 0f;
                    vertices[index++] = 1f;
                    vertices[index++] = 0f;
                }
            }

            currentVertices = vertices;
            currentIndices = GeneratePlaneIndices(w, h);
            setUp();
        }

        private float[] GenerateFlatPlane(int w, int h)
        {
            float[] verts = new float[w * h * 8];
            int index = 0;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float xpos = (x / (float)(w - 1)) - 0.5f;
                    float zpos = (y / (float)(h - 1)) - 0.5f;

                    // Position
                    verts[index++] = xpos;
                    verts[index++] = 0f;
                    verts[index++] = zpos;

                    // Texcoord
                    verts[index++] = x / (float)(w - 1);
                    verts[index++] = y / (float)(h - 1);

                    // Normal
                    verts[index++] = 0f;
                    verts[index++] = 1f;
                    verts[index++] = 0f;
                }
            }

            return verts;
        }

        private uint[] GeneratePlaneIndices(int w, int h)
        {
            var indices = new List<uint>();

            for (int y = 0; y < h - 1; y++)
            {
                for (int x = 0; x < w - 1; x++)
                {
                    uint topLeft = (uint)(y * w + x);
                    uint topRight = topLeft + 1;
                    uint bottomLeft = topLeft + (uint)w;
                    uint bottomRight = bottomLeft + 1;

                    indices.Add(topLeft);
                    indices.Add(bottomLeft);
                    indices.Add(topRight);

                    indices.Add(topRight);
                    indices.Add(bottomLeft);
                    indices.Add(bottomRight);
                }
            }

            return indices.ToArray();
        }

        public float GetHeightAt(float x, float z)
        {
            if (heightData == null)
                return 0f;

            float fx = ((x / scale.X) + 0.5f) * (width - 1);
            float fz = ((z / scale.Z) + 0.5f) * (height - 1);

            int ix = Math.Clamp((int)Math.Floor(fx), 0, width - 1);
            int iz = Math.Clamp((int)Math.Floor(fz), 0, height - 1);

            return heightData[iz * width + ix] * heightIntensity * scale.Y;
        }
    }
}
