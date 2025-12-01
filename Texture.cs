using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace Graphics
{
    public class Texture
    {
        private int handle;
        private TextureUnit boundUnit = TextureUnit.Texture0;
        public float AnisotropicLevel = 1.0f;
        public TextureWrapping Wrapping = TextureWrapping.Repeat;
        public int Handle => handle;
        public float[] borderColor = { 1.0f, 1.0f, 1.0f, 1.0f };

        private byte[] pixels;
        public int width;
        public int height;
        private int colorChannels;

        public Texture(string path, ushort unit = 0)
        {
            handle = GL.GenTexture();
            boundUnit = TextureUnit.Texture0 + unit;
            use();

            if (string.IsNullOrEmpty(path))
            {
                byte[] pixel = new byte[] { 255, 255, 255, 0 };
                Create(1, 1, true, pixel, unit);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                return;
            }

            string ext = Path.GetExtension(path).ToLower();
            if (ext == ".tga")
            {
                LoadTGA(path, unit);
            }

            ApplyWrapping();
            ApplyAnisotropy();
        }

        public void use()
        {
            GL.ActiveTexture(boundUnit);
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }

        public void SetSampling(TextureSampling sampling)
        {
            use();

            switch (sampling)
            {
                case TextureSampling.Nearest:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    break;
                case TextureSampling.bilinear:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    break;
                case TextureSampling.bilinear_mipmap:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapNearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    break;
                case TextureSampling.trilinear_mipmap:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    break;
            }

            ApplyAnisotropy();
        }

        public void SetWrapping(TextureWrapping wrapping)
        {
            Wrapping = wrapping;
            ApplyWrapping();
        }

        private void ApplyWrapping()
        {
            use();

            TextureWrapMode mode = TextureWrapMode.Repeat;

            switch (Wrapping)
            {
                case TextureWrapping.Repeat:
                    mode = TextureWrapMode.Repeat;
                    break;
                case TextureWrapping.MirroredRepeat:
                    mode = TextureWrapMode.MirroredRepeat;
                    break;
                case TextureWrapping.ClampToEdge:
                    mode = TextureWrapMode.ClampToEdge;
                    break;
                case TextureWrapping.ClampToBorder:
                    mode = TextureWrapMode.ClampToBorder;
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);
                    break;
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)mode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)mode);
        }

        private void ApplyAnisotropy()
        {
            float maxAniso = GL.GetFloat(GetPName.MaxTextureMaxAnisotropy);
            float clampedLevel = Math.Clamp(AnisotropicLevel, 1.0f, maxAniso);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxAnisotropy, clampedLevel);
        }

        public void Create(int width, int height, bool alpha, byte[] pixels, int unit)
        {
            this.width = width;
            this.height = height;
            this.pixels = pixels;
            this.colorChannels = alpha ? 4 : 3;

            boundUnit = TextureUnit.Texture0 + unit;
            GL.ActiveTexture(boundUnit);
            if (handle == 0) handle = GL.GenTexture();
            use();

            PixelInternalFormat internalFormat = alpha ? PixelInternalFormat.Rgba : PixelInternalFormat.Rgb;
            PixelFormat format = alpha ? PixelFormat.Rgba : PixelFormat.Rgb;

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, format, PixelType.UnsignedByte, pixels);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }

        public void LoadTGA(string filename, ushort unit)
        {
            if (!File.Exists(filename)) return;

            byte[] bytes = File.ReadAllBytes(filename);
            if (bytes.Length <= 18) return;

            TGAHeader header = new TGAHeader();
            header.idenSize = bytes[0];
            header.colorMapType = bytes[1];
            header.imageType = bytes[2];
            header.colorMapStart = (ushort)(bytes[3] | (bytes[4] << 8));
            header.colorMapLength = (ushort)(bytes[5] | (bytes[6] << 8));
            header.colorMapBits = bytes[7];
            header.xStart = (ushort)(bytes[8] | (bytes[9] << 8));
            header.yStart = (ushort)(bytes[10] | (bytes[11] << 8));
            header.width = (ushort)(bytes[12] | (bytes[13] << 8));
            header.height = (ushort)(bytes[14] | (bytes[15] << 8));
            header.pixelBits = bytes[16];
            header.imageDescriptor = bytes[17];

            colorChannels = header.pixelBits >> 3;
            bool alpha = colorChannels == 4;
            width = header.width;
            height = header.height;
            pixels = new byte[width * height * colorChannels];

            int pixelStart = 18 + header.idenSize;

            for (uint i = 0; i < width * height; i++)
            {
                int tgaIndex = pixelStart + (int)(i * colorChannels);
                int pixelIndex = (int)(i * colorChannels);

                if (colorChannels >= 3)
                {
                    pixels[pixelIndex + 0] = bytes[tgaIndex + 2];
                    pixels[pixelIndex + 1] = bytes[tgaIndex + 1];
                    pixels[pixelIndex + 2] = bytes[tgaIndex + 0];
                }

                if (colorChannels == 4)
                {
                    pixels[pixelIndex + 3] = bytes[tgaIndex + 3];
                }
            }

            Create(width, height, alpha, pixels, unit);
        }

        public void GenerateCheckeredTexture(ushort unit)
        {
            boundUnit = TextureUnit.Texture0 + unit;

            const int width = 8;
            const int height = 8;
            const byte O = 0;
            const byte I = 255;
            byte[] pixels = new byte[width * height * 3]{
                O,O,O, O,O,O, O,O,O, O,O,O, I,I,I, I,I,I, I,I,I, I,I,I,
                O,O,O, O,O,O, O,O,O, O,O,O, I,I,I, I,I,I, I,I,I, I,I,I,
                O,O,O, O,O,O, O,O,O, O,O,O, I,I,I, I,I,I, I,I,I, I,I,I,
                O,O,O, O,O,O, O,O,O, O,O,O, I,I,I, I,I,I, I,I,I, I,I,I,
                I,I,I, I,I,I, I,I,I, I,I,I, O,O,O, O,O,O, O,O,O, O,O,O,
                I,I,I, I,I,I, I,I,I, I,I,I, O,O,O, O,O,O, O,O,O, O,O,O,
                I,I,I, I,I,I, I,I,I, I,I,I, O,O,O, O,O,O, O,O,O, O,O,O,
                I,I,I, I,I,I, I,I,I, I,I,I, O,O,O, O,O,O, O,O,O, O,O,O};

            Create(width, height, false, pixels, unit);
        }

        public void Dispose()
        {
            GL.DeleteTexture(handle);
        }

        public bool GetPixel(float u, float v, byte[] rgba)
        {
            if (pixels == null || width <= 0 || height <= 0 || colorChannels <= 0)
                return false;

            int x = (int)((width - 1) * Math.Clamp(u, 0.0f, 1.0f));
            int y = (int)((height - 1) * Math.Clamp(v, 0.0f, 1.0f));

            int index = (y * width + x) * colorChannels;

            rgba[0] = colorChannels > 0 ? pixels[index + 0] : (byte)0;
            rgba[1] = colorChannels > 1 ? pixels[index + 1] : (byte)0;
            rgba[2] = colorChannels > 2 ? pixels[index + 2] : (byte)0;
            rgba[3] = colorChannels > 3 ? pixels[index + 3] : (byte)255;

            return true;
        }
    }
    public struct TGAHeader
    {
        public byte idenSize;
        public byte colorMapType;
        public byte imageType;
        public ushort colorMapStart;
        public ushort colorMapLength;
        public byte colorMapBits;
        public ushort xStart;
        public ushort yStart;
        public ushort width;
        public ushort height;
        public byte pixelBits;
        public byte imageDescriptor;
    }

    public enum TextureSampling
    {
        Nearest,
        bilinear,
        bilinear_mipmap,
        trilinear_mipmap
    }

    public enum TextureWrapping
    {
        Repeat,
        MirroredRepeat,
        ClampToEdge,
        ClampToBorder
    }

    public enum TextureType
    {
        Albedo,
        Normal,
        Light,
        Specular,
        Height
    }

}
