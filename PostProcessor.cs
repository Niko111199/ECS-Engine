using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Graphics
{
    public class PostProcessor
    {
        private int quadVAO;
        private int quadVBO;
        private Shader postShader;
        public PostProcessEffekt effekt;
        public Scene owner;

        public float brightness = 1.2f;
        public float contrast = 0.5f;
        public float shiftAmount = 1.0f;

        public PostProcessor(int width, int height, Scene owner)
        {
            float[] quadVertices = {
                // positions   // texCoords
                -1.0f,  1.0f,  0.0f, 1.0f,
                -1.0f, -1.0f,  0.0f, 0.0f,
                 1.0f, -1.0f,  1.0f, 0.0f,

                -1.0f,  1.0f,  0.0f, 1.0f,
                 1.0f, -1.0f,  1.0f, 0.0f,
                 1.0f,  1.0f,  1.0f, 1.0f
            };

            quadVAO = GL.GenVertexArray();
            quadVBO = GL.GenBuffer();
            GL.BindVertexArray(quadVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, quadVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

            postShader = new Shader(postVertex(), postFragment());
            this.owner = owner;
        }

        public void Render(int colorTexture)
        {
            GL.Disable(EnableCap.DepthTest);
            postShader.Use();

            GL.BindVertexArray(quadVAO);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, colorTexture);
            postShader.setInt("screenTexture", 0);
            postShader.setVec2("screenSize", new Vector2(owner.window.width, owner.window.height));
            postShader.setFloat("brightness", brightness);
            postShader.setFloat("contrast", contrast);
            postShader.setFloat("shiftAmount", shiftAmount);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

        public void Resize(int width, int height)
        {
            postShader.Use();
            postShader.setVec2("screenSize", new Vector2(width, height));
        }

        public void SetEffekt(PostProcessEffekt newEffekt)
        {
            effekt = newEffekt;

            postShader.Dispose();
            postShader = new Shader(postVertex(), postFragment());
            postShader.Use();
            postShader.setVec2("screenSize", new Vector2(owner.window.width, owner.window.height));
        }

        public string postVertex()
        {
            return @"
#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aTexCoords;

out vec2 TexCoords;

void main()
{
    TexCoords = aTexCoords;
    gl_Position = vec4(aPos, 0.0, 1.0);
}
";
        }

        public string postFragment()
        {
            string header = @"
#version 330 core
out vec4 FragColor;
in vec2 TexCoords;

uniform sampler2D screenTexture;
uniform vec2 screenSize;

uniform float brightness;
uniform float contrast;
uniform float shiftAmount;
";

            string main = @"
void main()
{
    vec3 color = texture(screenTexture, TexCoords).rgb;
";
            switch (effekt)
            {
                case PostProcessEffekt.none:
                    main += "vec3 Result = color;";
                    break;

                case PostProcessEffekt.inverted:
                    main += "vec3 Result = vec3(1.0) - color;";
                    break;

                case PostProcessEffekt.grayscale:
                    main += "float g = dot(color, vec3(0.2126, 0.7152, 0.0722)); vec3 Result = vec3(g);";
                    break;

                case PostProcessEffekt.sharpen:
                    main += KernelEffekt(sharpen);
                    break;

                case PostProcessEffekt.edge:
                    main += KernelEffekt(edge);
                    break;

                case PostProcessEffekt.brightness:
                    main += "vec3 Result = color * brightness;";
                    main += "Result = (Result - contrast) * 1.3 + contrast;";
                    break;

                case PostProcessEffekt.saturation:
                    main += "float grayscale = dot(color, vec3(.2126,.7152,.0722));";
                    main += "vec3 Result = mix(vec3(grayscale), color, 1.6);";
                    break;

                case PostProcessEffekt.sepia:
                    main += "vec3 Result = vec3(\r\n" +
                        "                       dot(color, vec3(0.393, 0.769, 0.189)),\r\n" +
                        "                       dot(color, vec3(0.349, 0.686, 0.168)),\r\n" +
                        "                       dot(color, vec3(0.272, 0.534, 0.131))\r\n); ";
                    break;

                case PostProcessEffekt.posterize:
                    main += "float levels = 8.0;";
                    main += "vec3 Result = floor(color * levels) / levels;";
                    break;

                case PostProcessEffekt.chromaticAberration:
                    main += "float shift = shiftAmount / screenSize.x;";
                    main += "float r = texture(screenTexture, TexCoords + vec2( shift, 0)).r;";
                    main += "float g = texture(screenTexture, TexCoords).g;";
                    main += "float b = texture(screenTexture, TexCoords - vec2( shift, 0)).b;";
                    main += "vec3 Result = vec3(r,g,b);";
                    break;

                case PostProcessEffekt.scanlines:
                    main += "float scan = sin(TexCoords.y * screenSize.y * 4.0) * 0.04;";
                    main += "vec3 Result = color - scan;";
                    break;

                case PostProcessEffekt.glitch:
                    main += "float t = fract(TexCoords.y * 20.0);";
                    main += "vec2 uv = TexCoords;";
                    main += "uv.x += (step(0.95, t) * 0.05);";
                    main += "vec3 Result = texture(screenTexture, uv).rgb;";
                    break;

                case PostProcessEffekt.nightVision:
                    main += "float gray = dot(color, vec3(0.299, 0.587, 0.114));";
                    main += "float noise = fract(sin(dot(TexCoords * vec2(800.0, 600.0), vec2(12.9898,78.233))) * 43758.5453);";
                    main += "vec3 Result = vec3(gray * 0.8 + noise * 0.2) * vec3(0.1, 1.0, 0.1);";
                    break;

                default:
                    main += "vec3 Result = color;";
                    break;
            }
            main += @"
    FragColor = vec4(Result, 1.0);
}
";

            return header + main;
        }

        private string KernelEffekt(float[,] kernel)
        {
            string code = "vec3 Result = vec3(0.0);\n";

            code += "vec2 tex_offset = 1.0 / screenSize;\n";

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    float value = kernel[y + 1, x + 1];
                    code += $"Result += texture(screenTexture, TexCoords + vec2({x} * tex_offset.x, {y} * tex_offset.y)).rgb * {value};\n";
                }
            }

            return code;
        }


        private readonly float[,] sharpen =
        {
            {  0, -1,  0 },
            { -1,  5, -1 },
            {  0, -1,  0 }
        };

        public readonly float[,] edge =
        {
            { -1, -1, -1 },
            { -1,  8, -1 },
            { -1, -1, -1 }
        };

        public readonly float[,] blur =
        {
            {1/9f, 1/9f, 1/9f},
            {1/9f, 1/9f, 1/9f},
            {1/9f, 1/9f, 1/9f}
        };
    }
}

public enum PostProcessEffekt
{
    none,
    inverted,
    grayscale,
    sharpen,
    edge,
    brightness,
    saturation,
    sepia,
    posterize,
    chromaticAberration,
    scanlines,
    glitch,
    nightVision
}