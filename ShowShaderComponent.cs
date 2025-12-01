
namespace Graphics
{
    public class ShowShaderComponent: IComponent
    {
        private Entity owner; 
        public ShowShaderComponent(Entity owner)
        {
            this.owner = owner;
        }

        public string printFragmentShader()
        {
            return owner.FragmentShader();
        }

        public string printVertexShader()
        {
            return owner.VertexShader();
        }
    }
}
