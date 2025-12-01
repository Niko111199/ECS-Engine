using OpenTK.Mathematics;

namespace Graphics
{
    internal class ShowNormalsComponenet : IComponent
    {
        private Shader normalShader;
        private Entity owner;
        public bool draw;

        public ShowNormalsComponenet(Entity owner)
        {
            this.owner = owner;
            normalShader = new Shader(Vertex(), Fragment(), Geometry(), owner);
        }

        public void Draw(Entity entity, Matrix4 view, Matrix4 projection)
        {
            if (draw)
            {
                var transform = entity.GetComponent<TransformComponent>();
                Matrix4 model = transform != null ? transform.GetModelMatrix() : Matrix4.Identity;

                normalShader.Use();
                normalShader.setMat4("model", model);
                normalShader.setMat4("view", view);
                normalShader.setMat4("projection", projection);
                normalShader.setFloat("normalLength", 0.1f);
                normalShader.setVec3("normalColor", new Vector3(1, 1, 0));

                var mesh = entity.GetComponent<MeshComponent>();
                if (mesh != null)
                {
                    mesh.Draw();
                }
            }
        }

        private string Vertex()
        {
            return @"
#version 330 core
layout(location = 0) in vec3 aPos;
layout(location = 2) in vec3 aNormal;

out VS_OUT {
    vec3 normal;
    vec3 pos;
} vs_out;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    vec4 worldPos = model * vec4(aPos, 1.0);
    vs_out.pos = worldPos.xyz;
    vs_out.normal = mat3(transpose(inverse(model))) * aNormal;
    gl_Position = projection * view * worldPos;
}";
        }

        private string Geometry()
        {
            return @"
#version 330 core
layout (triangles) in;
layout (line_strip, max_vertices = 6) out;

in VS_OUT {
    vec3 normal;
    vec3 pos;
} gs_in[];

uniform mat4 view;
uniform mat4 projection;
uniform float normalLength;

void main()
{
    for(int i = 0; i < 3; i++)
    {
        vec3 start = gs_in[i].pos;
        vec3 end = start + normalize(gs_in[i].normal) * normalLength;

        gl_Position = projection * view * vec4(start, 1.0);
        EmitVertex();

        gl_Position = projection * view * vec4(end, 1.0);
        EmitVertex();

        EndPrimitive();
    }
}";
        }

        private string Fragment()
        {
            return @"
#version 330 core
out vec4 FragColor;
uniform vec3 normalColor;
void main()
{
    FragColor = vec4(normalColor, 1.0);
}";
        }
    }
}
