using FormatKFIV.Asset;
using SharpGLTF.Materials;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GltfKFIV.Gltf;

internal static class Textures
{
    private static byte[] PngFromRgba(Texture.ImageBuffer image, byte[] data)
    {
        var width = (int)image.Width;
        var height = (int)image.Height;

        using var img = Image.LoadPixelData<Bgra32>(data, width, height);
        var memoryStream = new MemoryStream();
        img.SaveAsPng(memoryStream);
        return memoryStream.GetBuffer();
    }

    public static Dictionary<uint, MaterialBuilder> GenerateTextures(Scene scene)
    {
        var texturesByGuid = new Dictionary<uint, MaterialBuilder>();
        foreach (var tex in scene.texData)
        {
            for (var i = 0; i < tex.SubimageCount; ++i)
            {
                var subImage = tex.GetSubimage(i);
                if (subImage != null)
                {
                    // Console.WriteLine("IMAGE " + subImage.Format);
                    var rgba = tex.GetSubimageAsRGBA(i);
                    var memoryPng = PngFromRgba(subImage.Value, rgba);

                    var material = new MaterialBuilder()
                        .WithDoubleSide(true)
                        .WithBaseColor(memoryPng);

                    texturesByGuid[subImage.Value.UID] = material;
                }
            }
        }

        return texturesByGuid;
    }
}