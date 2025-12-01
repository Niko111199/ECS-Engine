using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;

namespace Graphics
{
    public class SkyboxComponent : IComponent
    {
        private Entity owner;
        private CubemapTexture cubemap;
        private int vao, vbo;

        private string[] facePaths = new string[6];
        private int loadedFaces = 0;

        private static readonly float[] skyboxVertices = {
            -1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            -1.0f,  1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f,  1.0f
        };

        public SkyboxComponent(Entity owner)
        {
            this.owner = owner;
        }

        public void SetFace(int index, string path)
        {
            if (index < 0 || index >= 6) return;
            facePaths[index] = path;
            loadedFaces++;

            if (loadedFaces == 6)
            {
                Setup(facePaths);
            }
        }

        private void Setup(string[] paths)
        {
            cubemap = new CubemapTexture(paths);

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, skyboxVertices.Length * sizeof(float), skyboxVertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        }

        public void Bind(TextureUnit unit)
        {
            if (cubemap != null)
                cubemap.Use(unit);
        }

        public void Draw(Shader shader, Matrix4 view, Matrix4 projection)
        {
            if (cubemap == null) return;

            GL.DepthFunc(DepthFunction.Lequal);

            shader.Use();
            Matrix4 viewNoTranslation = new Matrix4(new Matrix3(view));
            shader.setMat4("view", viewNoTranslation);
            shader.setMat4("projection", projection);

            cubemap.Use(TextureUnit.Texture0);
            shader.setInt("skybox", 0);

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);

            GL.DepthFunc(DepthFunction.Less);
        }

        public string VertexShader()
        {
            return @"
#version 330 core
layout(location = 0) in vec3 aPos;

out vec3 TexCoord;

uniform mat4 view;
uniform mat4 projection;

void main()
{
    TexCoord = aPos;
    vec4 pos = projection * view * vec4(aPos, 1.0);
    gl_Position = pos.xyww; // depth = 1.0
}
";
        }

        public string FragmenShader()
        {
            return @"
#version 330 core
out vec4 FragColor;

in vec3 TexCoord;

uniform samplerCube skybox;

void main()
{
    FragColor = texture(skybox, -TexCoord);
}
";
        }
    }
}
