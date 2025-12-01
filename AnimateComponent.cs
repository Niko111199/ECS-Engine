
namespace Graphics
{
    internal class AnimateComponent : IComponent
    {
        private Entity owner;
        public float time;
        public float speed;
        public float Amplitude;

        public AnimateComponent(Entity owner)
        {
            this.owner = owner;
        }

        public void Update(float deltaTime)
        {
            time += deltaTime * speed;
        }
    }
}
