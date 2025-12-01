using OpenTK.Mathematics;

namespace Graphics
{
    public class TextureProjectorComponent : IComponent
    {
        private readonly Entity owner;
        public Texture Texture { get; private set; }
        public string Path { get; private set; }
        public float Fov { get; set; } = 45f;
        public float AspectRatio { get; set; } = 1.0f;
        public float Near { get; set; } = 0.1f;
        public float Far { get; set; } = 100f;

        public TextureProjectorComponent(Entity owner)
        {
            this.owner = owner;
        }

        public void LoadTexture(string path, ushort unit = 4)
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

        public void Bind() => Texture?.use();

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(Fov), AspectRatio, Near, Far);
        }

        public Matrix4 GetViewMatrix()
        {
            var transform = owner.GetComponent<TransformComponent>();
            if (transform == null)
            {
                return Matrix4.Identity;
            }

            var rot =
                Matrix4.CreateRotationX(transform.Rotation.X) *
                Matrix4.CreateRotationY(transform.Rotation.Y) *
                Matrix4.CreateRotationZ(transform.Rotation.Z);

            Vector3 forward = Vector3.TransformVector(-Vector3.UnitZ, rot);
            return Matrix4.LookAt(transform.Position, transform.Position + forward, Vector3.UnitY);
        }
    }
}
