namespace Graphics
{
    internal class GrassComponent : MeshComponent
    {
        public override string Name => "terrainObj";

        private float height = 1f;
        private float width = 1f;
        private Entity owner;

        public GrassComponent(Entity owner)
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
            float halfWidth = width / 2f;

            return new float[]
            {
                // Plane 1
                -halfWidth, 0, 0,      0,0,   0,1,0,
                 halfWidth, 0, 0,      1,0,   0,1,0,
                 halfWidth, height,0,  1,1,   0,1,0,
                -halfWidth, height,0,  0,1,   0,1,0,

                // Plane 2
                0, 0, -halfWidth,      0,0,   0,1,0,
                0, 0,  halfWidth,      1,0,   0,1,0,
                0, height, halfWidth,  1,1,   0,1,0,
                0, height,-halfWidth,  0,1,   0,1,0,

                // Plane 3
               -halfWidth, 0, halfWidth,       0,0,   0,1,0,
                halfWidth, 0,-halfWidth,       1,0,   0,1,0,
                halfWidth, height,-halfWidth,  1,1,   0,1,0,
               -halfWidth, height, halfWidth,  0,1,   0,1,0,
            };
        }

        protected override uint[] GetIndices()
        {
            return new uint[]
            {
                0,1,2, 2,3,0,       // Plane 1
                4,5,6, 6,7,4,       // Plane 2
                8,9,10, 10,11,8     // Plane 3
            };
        }
    }
}
