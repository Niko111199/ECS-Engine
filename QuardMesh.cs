using Graphics;

internal class QuadMesh : MeshComponent
{
    public QuadMesh(Entity owner)
    {
        this.owner = owner;
        baseVertices = GetVertices();
        baseIndices = GetIndices();

        currentVertices = (float[])baseVertices.Clone();
        currentIndices = (uint[])baseIndices.Clone();
        setUp();
    }

    public override string Name => "Quad";

    protected override float[] GetVertices() => new float[]
    {
        //  x,     y,     z,     u,     v,      nx,   ny,    nz
        -0.5f,  0.5f,  0.0f,    0.0f, 1.0f,    0.0f,  0.0f,  1.0f,
         0.5f,  0.5f,  0.0f,    1.0f, 1.0f,    0.0f,  0.0f,  1.0f,
        -0.5f, -0.5f,  0.0f,    0.0f, 0.0f,    0.0f,  0.0f,  1.0f,
         0.5f, -0.5f,  0.0f,    1.0f, 0.0f,    0.0f,  0.0f,  1.0f
    };

    protected override uint[] GetIndices() => new uint[]
    {
        0, 2, 1,
        1, 2, 3
    };
}
