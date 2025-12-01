using OpenTK.Graphics.OpenGL4;

namespace Graphics
{
    public class FrameBuffer
    {
        public int FBO;
        public int ColorTexture;
        public int DepthRBO;

        public FrameBuffer(int width, int height)
        {
            FBO = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);

            ColorTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                          width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                                    FramebufferAttachment.ColorAttachment0,
                                    TextureTarget.Texture2D, ColorTexture, 0);


            DepthRBO = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthRBO);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, width, height);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                                       FramebufferAttachment.DepthStencilAttachment,
                                       RenderbufferTarget.Renderbuffer, DepthRBO);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
                throw new Exception($"Framebuffer ikke komplet: {status}");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Resize(int width, int height)
        {
            GL.DeleteTexture(ColorTexture);
            GL.DeleteRenderbuffer(DepthRBO);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);

            ColorTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                          width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                                    FramebufferAttachment.ColorAttachment0,
                                    TextureTarget.Texture2D, ColorTexture, 0);

            DepthRBO = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthRBO);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, width, height);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                                       FramebufferAttachment.DepthStencilAttachment,
                                       RenderbufferTarget.Renderbuffer, DepthRBO);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }


        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}
