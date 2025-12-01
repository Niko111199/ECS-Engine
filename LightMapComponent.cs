
namespace Graphics
{
    public class LightMapComponent : IComponent
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

        public LightMapComponent(Entity owner)
        {
            this.owner = owner;
            LoadTexture();
        }

        public void LoadTexture(string path = "", ushort unit = 3)
        {
            Texture?.Dispose();
            Path = path;

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                Texture = new Texture(path, unit);
            else
            {
                Texture = new Texture("", unit);
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
            Texture?.use();
        }

        public void Dispose()
        {
            Texture.Dispose();
        }
    }
}
