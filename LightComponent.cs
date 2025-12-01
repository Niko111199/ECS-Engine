using OpenTK.Mathematics;

namespace Graphics
{
    public class LightComponent : IComponent
    {
        private readonly Entity owner;

        public LightType Type { get; set; } = LightType.Directional;
        public Vector3 Color { get; set; } = new Vector3(1f, 1f, 1f);
        public float Intensity { get; set; } = 1.0f;

        public float Range { get; set; } = 10.0f;

        public float InnerCutoff { get; set; } = MathHelper.DegreesToRadians(12.5f);
        public float OuterCutoff { get; set; } = MathHelper.DegreesToRadians(17.5f);

        public LightComponent(Entity owner)
        {
            this.owner = owner;
        }

        public string Name => "Light";
    }
        public enum LightType
        {
            Directional,
            Point,
            Spot
        }
}
