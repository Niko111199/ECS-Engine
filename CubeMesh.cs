
namespace Graphics
{
    internal class CubeMesh : MeshComponent
    {
        public CubeMesh(Entity owner)
        {
            this.owner = owner;
            baseVertices = GetVertices();
            baseIndices = GetIndices();

            currentVertices = (float[])baseVertices.Clone();
            currentIndices = (uint[])baseIndices.Clone();
            setUp();
        }

        public override string Name => "Cube";

        protected override float[] GetVertices() => new float[]
        {
            //  x,     y,     z,        u,     v        nx,    ny,    nz
              -0.5f, -0.5f,  0.5f,     0.0f,  0.0f,     0.0f,  0.0f,  1.0f, 
               0.5f, -0.5f,  0.5f,     1.0f,  0.0f,     0.0f,  0.0f,  1.0f,      // Front face
               0.5f,  0.5f,  0.5f,     1.0f,  1.0f,     0.0f,  0.0f,  1.0f,
              -0.5f,  0.5f,  0.5f,     0.0f,  1.0f,     0.0f,  0.0f,  1.0f,

              -0.5f, -0.5f, -0.5f,     1.0f,  0.0f,     0.0f,  0.0f, -1.0f,
               0.5f, -0.5f, -0.5f,     0.0f,  0.0f,     0.0f,  0.0f, -1.0f,      // Back face
               0.5f,  0.5f, -0.5f,     0.0f,  1.0f,     0.0f,  0.0f, -1.0f,
              -0.5f,  0.5f, -0.5f,     1.0f,  1.0f,     0.0f,  0.0f, -1.0f,

              -0.5f, -0.5f, -0.5f,     0.0f,  0.0f,    -1.0f,  0.0f,  0.0f,
              -0.5f, -0.5f,  0.5f,     1.0f,  0.0f,    -1.0f,  0.0f,  0.0f,      // Left face
              -0.5f,  0.5f,  0.5f,     1.0f,  1.0f,    -1.0f,  0.0f,  0.0f,
              -0.5f,  0.5f, -0.5f,     0.0f,  1.0f,    -1.0f,  0.0f,  0.0f,

               0.5f, -0.5f, -0.5f,     1.0f,  0.0f,     1.0f,  0.0f,  0.0f,
               0.5f, -0.5f,  0.5f,     0.0f,  0.0f,     1.0f,  0.0f,  0.0f,      // Right face
               0.5f,  0.5f,  0.5f,     0.0f,  1.0f,     1.0f,  0.0f,  0.0f,
               0.5f,  0.5f, -0.5f,     1.0f,  1.0f,     1.0f,  0.0f,  0.0f,

              -0.5f, -0.5f, -0.5f,     0.0f,  1.0f,     0.0f, -1.0f,  0.0f,
               0.5f, -0.5f, -0.5f,     1.0f,  1.0f,     0.0f, -1.0f,  0.0f,      // Bottom face
               0.5f, -0.5f,  0.5f,     1.0f,  0.0f,     0.0f, -1.0f,  0.0f,
              -0.5f, -0.5f,  0.5f,     0.0f,  0.0f,     0.0f, -1.0f,  0.0f,

              -0.5f,  0.5f, -0.5f,     0.0f, 1.0f,      0.0f,  1.0f,  0.0f,
               0.5f,  0.5f, -0.5f,     1.0f, 1.0f,      0.0f,  1.0f,  0.0f,      // Top face
               0.5f,  0.5f,  0.5f,     1.0f, 0.0f,      0.0f,  1.0f,  0.0f,
               -0.5f,  0.5f,  0.5f,    0.0f, 0.0f,      0.0f,  1.0f,  0.0f

        };

        protected override uint[] GetIndices() => new uint[]
{
            // Back face
            0, 1, 2,
            0, 2, 3,

            

            // Front face
            4, 6, 5,
            4, 7, 6,
           

            // Left face
            8, 9, 10,
            8, 10, 11,
           

            // Right face
            12, 14, 13,
            12, 15, 14,
            

            // Top face
            16, 17, 18,
            16, 18, 19,
            

            // Bottom face
            20, 22, 21,
            20, 23, 22

};
    }
}
