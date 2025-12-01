using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Graphics
{
    internal abstract class MeshComponent : IComponent
    {
        protected int vao;
        protected int vbo;
        protected int ebo;
        protected Entity owner;
        public abstract string Name { get; }

        protected float[] currentVertices;
        protected uint[] currentIndices;
        protected int indexCount;

        protected float[] baseVertices;
        protected uint[] baseIndices;

        public Vector4 color;

        protected abstract float[] GetVertices();
        protected abstract uint[] GetIndices();

        public void setUp()
        {
            color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

            float[] vertices = currentVertices;
            uint[] indices = currentIndices;

            indexCount = indices.Length;

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            int stride = 8 * sizeof(float);
            // position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            // texcoord
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // normals
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, 5 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }

        public virtual void Draw()
        {
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
        }

        public virtual void Dispose()
        {
            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);
        }
    }
}
