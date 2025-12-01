using Graphics;
using OpenTK.Graphics.OpenGL4;

public class ShadowMap
{
    public int FBO;
    public int DepthTexture;
    public int Width, Height;
    public Shader shadowShader;
    public float bias;

    public ShadowMap(int width, int height)
    {
        Width = width;
        Height = height;

        FBO = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);

        DepthTexture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, DepthTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent,
                      width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

        float[] borderColor = { 1.0f, 1.0f, 1.0f, 1.0f };
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                                FramebufferAttachment.DepthAttachment,
                                TextureTarget.Texture2D, DepthTexture, 0);

        GL.DrawBuffer(DrawBufferMode.None);
        GL.ReadBuffer(ReadBufferMode.None);

        var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        if (status != FramebufferErrorCode.FramebufferComplete)
            throw new Exception($"Shadow framebuffer ikke komplet: {status}");

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        shadowShader = new Shader(ShadowVertex(), ShadowFragment());

        bias = 0.0005f;
    }

    public void Bind()
    {
        GL.Viewport(0, 0, Width, Height);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
        GL.Clear(ClearBufferMask.DepthBufferBit);
    }

    public void Unbind(int screenWidth, int screenHeight)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.Viewport(0, 0, screenWidth, screenHeight);
    }

    public string ShadowVertex()
    {
        return @"
#version 330 core
layout(location = 0) in vec3 aPos;

uniform mat4 model;
uniform mat4 lightSpaceMatrix;

void main()
{
    gl_Position = lightSpaceMatrix * model * vec4(aPos, 1.0);
}
";
    }

    public string ShadowFragment()
    {
        return @"
#version 330 core
void main()
{

}
";
    }
}
