using OpenTK.Mathematics;

namespace Graphics
{
    public class CameraComponent : IComponent
    {
        private readonly Entity owner;

        public Camera Camera { get; private set; }

        public CameraComponent(Entity owner)
        {
            this.owner = owner;

            setUp(owner.aspectRatio);
        }

        public void setUp(float aspectRatio)
        {
            var transform = owner.GetComponent<TransformComponent>();
            Vector3 pos = transform != null ? transform.Position : Vector3.Zero;

            Camera = new Camera(pos, aspectRatio);
        }

        public string Name => "Camera";
    }
}
