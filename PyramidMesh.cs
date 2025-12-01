
namespace Graphics
{
    internal class PyramidMesh : MeshComponent
    {
        public PyramidMesh(Entity owner)
        {
            this.owner = owner;
            baseVertices = GetVertices();
            baseIndices = GetIndices();

            currentVertices = (float[])baseVertices.Clone();
            currentIndices = (uint[])baseIndices.Clone();
            setUp();
        }

        public override string Name => "Pyramid";

        protected override float[] GetVertices() => new float[]
        {
          //  x,     y,     z,       u,     v,       nx,    ny,    nz
            -0.5f,  0.0f, -0.5f,    0.0f,  0.0f,    0.0f,  -1.0f,  0.0f, 
             0.5f,  0.0f, -0.5f,    1.0f,  0.0f,    0.0f,  -1.0f,  0.0f,       // base
             0.5f,  0.0f,  0.5f,    1.0f,  1.0f,    0.0f,  -1.0f,  0.0f,
            -0.5f,  0.0f,  0.5f,    0.0f,  1.0f,    0.0f,  -1.0f,  0.0f,

             0.0f,  1.0f,  0.0f,    0.5f,  1.0f,    0f, 0.707f, -0.707f,
             0.5f,  0.0f, -0.5f,    1.0f,  0.0f,    0f, 0.707f, -0.707f,       // side 1
            -0.5f,  0.0f, -0.5f,    0.0f,  0.0f,    0f, 0.707f, -0.707f,

             0.0f,  1.0f,  0.0f,    0.5f,  1.0f,    0.707f, 0.707f, 0f,
             0.5f,  0.0f,  0.5f,    1.0f,  0.0f,    0.707f, 0.707f, 0f,        // side 2
             0.5f,  0.0f, -0.5f,    0.0f,  0.0f,    0.707f, 0.707f, 0f,

             0.0f,  1.0f,  0.0f,    0.5f,  1.0f,    0f, 0.707f, 0.707f,
            -0.5f,  0.0f,  0.5f,    1.0f,  0.0f,    0f, 0.707f, 0.707f,        // side 3
             0.5f,  0.0f,  0.5f,    0.0f,  0.0f,    0f, 0.707f, 0.707f,

             0.0f,  1.0f,  0.0f,    0.5f,  1.0f,    -0.707f, 0.707f, 0f,
            -0.5f,  0.0f, -0.5f,    1.0f,  0.0f,    -0.707f, 0.707f, 0f,       // side 4
            -0.5f,  0.0f,  0.5f,    0.0f,  0.0f,    -0.707f, 0.707f, 0f,
        };

        protected override uint[] GetIndices() => new uint[]
        {
              // Base
            0,  1, 2,
            0,  2, 3,

            // Sides
            4,  5,  6,
            7,  8,  9,
            10, 11, 12,
            13, 14, 15
        };
    }
}
