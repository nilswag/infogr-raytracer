using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;
using Template;

namespace INFOGRTemplate
{
    public class Texture
    {

        private static Image<Rgba32> defaultTexture = Image.Load<Rgba32>("assets/default.png");

        private Image<Rgba32> image { get; set; }

        public Texture(string path)
        {
            image = Image.Load<Rgba32>(path);
        }

        public Texture()
        {
            image = defaultTexture;
        }

        public Color3 GetPixel(float u, float v)
        {
            int x = (int)(u * image.Width);
            int y = (int)(v * image.Height);

            Rgba32 pixel = image[x, y];

            return new Color3(
                pixel.R / 255.0f,
                pixel.G / 255.0f,
                pixel.B / 255.0f
            );
        }

    }
}
