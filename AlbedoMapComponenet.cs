using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Graphics
{
    public class AlbedoMapComponenet : IComponent
    {
        private Entity owner;
        public Texture Texture { get; private set; }
        public string Path { get; private set; }

        public TextureWrapping Wrapping
        {
            get => Texture.Wrapping;
            set => Texture.SetWrapping(value);
        }

        public TextureSampling Sampling { get; private set; } = TextureSampling.bilinear;
        public float AnisotropicLevel
        {
            get => Texture.AnisotropicLevel;
            set
            {
                Texture.AnisotropicLevel = value;
                Texture.SetSampling(Sampling);
            }
        }

        public Vector2 UVScale { get; set; } = new Vector2(1.0f, 1.0f);

        public AlbedoMapComponenet(Entity owner)
        {
            this.owner = owner;
            LoadTexture();
        }

        public void LoadTexture(string path = "", TextureType unit = TextureType.Albedo)
        {
            Texture?.Dispose();
            Path = path;

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                Texture = new Texture(path, unit);
            }
            else
            {
                Texture = new Texture("", unit);
                Texture.GenerateCheckeredTexture((ushort)unit);
            }
            Bind();
        }

        public void SetSampling(TextureSampling sampling)
        {
            Sampling = sampling;
            Texture.SetSampling(sampling);
        }

        public void Bind()
        {
            if (Texture != null)
                Texture.use();
        }

        public void Dispose()
        {
            Texture.Dispose();
        }
    }
}
