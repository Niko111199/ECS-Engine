using System.Numerics;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Graphics
{
    internal class OBJLoader
    {
        public Vector3[] Vertices;
        public Vector2[] Uvs;
        public Vector3[] Normals;

        public OBJLoader(string path)
        {
            LoadOBJ(path);
        }

        private void LoadOBJ(string path)
        {
            var lines = File.ReadAllLines(path);

            var tempVertices = new List<Vector3>();
            var tempUvs = new List<Vector2>();
            var tempNormals = new List<Vector3>();

            var vertexIndices = new List<int>();
            var uvIndices = new List<int>();
            var normalIndices = new List<int>();

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                switch (parts[0])
                {
                    case "v":
                        tempVertices.Add(new Vector3(
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture),
                            float.Parse(parts[3], CultureInfo.InvariantCulture)
                        ));
                        break;

                    case "vt":
                        tempUvs.Add(new Vector2(
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture)
                        ));
                        break;

                    case "vn":
                        tempNormals.Add(new Vector3(
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture),
                            float.Parse(parts[3], CultureInfo.InvariantCulture)
                        ));
                        break;

                    case "f":
                        for (int i = 2; i < parts.Length - 1; i++)
                        {
                            AddVertex(parts[1], tempVertices, tempUvs, tempNormals, vertexIndices, uvIndices, normalIndices);
                            AddVertex(parts[i], tempVertices, tempUvs, tempNormals, vertexIndices, uvIndices, normalIndices);
                            AddVertex(parts[i + 1], tempVertices, tempUvs, tempNormals, vertexIndices, uvIndices, normalIndices);
                        }
                        break;
                }
            }

            Vertices = new Vector3[vertexIndices.Count];
            Uvs = new Vector2[uvIndices.Count];
            Normals = new Vector3[normalIndices.Count];

            for (int i = 0; i < vertexIndices.Count; i++)
            {
                Vertices[i] = tempVertices[vertexIndices[i]];
                Uvs[i] = uvIndices[i] >= 0 ? tempUvs[uvIndices[i]] : Vector2.Zero;
                Normals[i] = normalIndices[i] >= 0 ? tempNormals[normalIndices[i]] : Vector3.UnitY;
            }
        }

        private void AddVertex(string faceVertex, List<Vector3> tempVertices, List<Vector2> tempUvs, List<Vector3> tempNormals,
            List<int> vertexIndices, List<int> uvIndices, List<int> normalIndices)
        {
            var indices = faceVertex.Split('/');

            int vIndex = int.Parse(indices[0]) - 1;
            int uvIndex = (indices.Length > 1 && !string.IsNullOrEmpty(indices[1])) ? int.Parse(indices[1]) - 1 : -1;
            int nIndex = (indices.Length > 2 && !string.IsNullOrEmpty(indices[2])) ? int.Parse(indices[2]) - 1 : -1;

            vertexIndices.Add(vIndex);
            uvIndices.Add(uvIndex);
            normalIndices.Add(nIndex);
        }
    }
}
