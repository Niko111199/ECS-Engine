using OpenTK.Mathematics;

namespace Graphics
{
    public class TransformComponent : IComponent
    {
        private Vector3 position;
        private Vector3 rotation;
        private Vector3 scale = Vector3.One;
        private readonly Entity owner;

        public Matrix4 LocalMatrix =>
    Matrix4.CreateScale(scale) *
    Matrix4.CreateRotationX(rotation.X) *
    Matrix4.CreateRotationY(rotation.Y) *
    Matrix4.CreateRotationZ(rotation.Z) *
    Matrix4.CreateTranslation(position);

        public Matrix4 GetWorldMatrix()
        {
            var local = LocalMatrix;

            if (owner.parent != null)
            {
                var parentTransform = owner.parent.GetComponent<TransformComponent>();
                if (parentTransform != null)
                {
                    return parentTransform.GetWorldMatrix() * local;
                }
            }

            return local;
        }

        public TransformComponent(Entity owner)
        {
            this.owner = owner;

            UpdateallChildrenTransforms();
        }

        public Vector3 Position
        {
            get => position;
            set
            {
                position = value;
                Console.WriteLine($"entity {owner.Id} Position changed to {position}");
            }
        }

        public Vector3 Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                Console.WriteLine($"entity {owner.Id} Rotation changed to {rotation}");
            }
        }

        public Vector3 Scale
        {
            get => scale;
            set
            {
                scale = value;
                Console.WriteLine($"entity {owner.Id} Scale changed to {scale}");
            }
        }

        public Vector3 Forward
        {
            get
            {
                var q = Quaternion.FromEulerAngles(Rotation);
                return Vector3.Transform(-Vector3.UnitZ, q);
            }
        }

        public void UpdateallChildrenTransforms()
        {
            foreach (Entity child in owner.Children)
            {
                child.GetComponent<TransformComponent>().GetWorldMatrix();
            }
        }

        public Matrix4 GetModelMatrix()
        {
            return Matrix4.CreateScale(scale) *
                   Matrix4.CreateRotationX(rotation.X) *
                   Matrix4.CreateRotationY(rotation.Y) *
                   Matrix4.CreateRotationZ(rotation.Z) *
                   Matrix4.CreateTranslation(position);
        }
    }
}
