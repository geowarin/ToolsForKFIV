using System.IO;
using FormatKFIV.Asset;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ToolsForKFIV.Gltf;

static internal class Textures
{
    public static byte[] PngFromRgba(Texture.ImageBuffer image, byte[] data)
    {
        var width = (int)image.Width;
        var height = (int)image.Height;

        using var img = Image.LoadPixelData<Rgba32>(data, width, height);
        var memoryStream = new MemoryStream();
        img.SaveAsPng(memoryStream);
        return memoryStream.GetBuffer();
    }
}