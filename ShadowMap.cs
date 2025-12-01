using OpenTK.Graphics.OpenGL4;

public class ShadowMap
{
    public int FBO;
    public int DepthTexture;
    public int Width, Height;

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

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
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
}
