using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;

namespace Graphics
{
    public class CubemapTexture
    {
        public int Handle { get; private set; }

        public CubemapTexture(string[] facePaths)
        {
            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, Handle);

            for (int i = 0; i < 6; i++)
            {
                if (File.Exists(facePaths[i]))
                {
                    byte[] bytes = File.ReadAllBytes(facePaths[i]);
                    LoadTGAIntoCubemapFace(bytes, TextureTarget.TextureCubeMapPositiveX + i);
                }
                else
                {
                    Console.WriteLine($"Missing cubemap face: {facePaths[i]}");
                }
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
        }

        private void LoadTGAIntoCubemapFace(byte[] bytes, TextureTarget target)
        {
            if (bytes.Length < 18) return;

            ushort width = (ushort)(bytes[12] | (bytes[13] << 8));
            ushort height = (ushort)(bytes[14] | (bytes[15] << 8));
            byte bpp = bytes[16];
            int pixelSize = bpp / 8;
            bool alpha = pixelSize == 4;
            byte[] pixels = new byte[width * height * pixelSize];

            int pixelStart = 18 + bytes[0];
            for (uint i = 0; i < width * height; i++)
            {
                int src = pixelStart + (int)(i * pixelSize);
                int dst = (int)(i * pixelSize);
                pixels[dst + 0] = bytes[src + 2];
                pixels[dst + 1] = bytes[src + 1];
                pixels[dst + 2] = bytes[src + 0];
                if (pixelSize == 4)
                    pixels[dst + 3] = bytes[src + 3];
            }

            PixelInternalFormat internalFmt = alpha ? PixelInternalFormat.Rgba : PixelInternalFormat.Rgb;
            PixelFormat fmt = alpha ? PixelFormat.Rgba : PixelFormat.Rgb;

            GL.TexImage2D(target, 0, internalFmt, width, height, 0, fmt, PixelType.UnsignedByte, pixels);
        }

        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.TextureCubeMap, Handle);
        }
    }
}
