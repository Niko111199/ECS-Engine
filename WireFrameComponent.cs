
namespace Graphics
{
    internal class WireFrameComponent : IComponent
    {
        public bool RenderAsWire;
        public Entity owner;

        public WireFrameComponent(Entity owner)
        {
            this.owner = owner;
        }
    }
}
