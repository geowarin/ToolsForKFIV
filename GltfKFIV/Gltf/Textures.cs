using FormatKFIV.Asset;
using SharpGLTF.Materials;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GltfKFIV.Gltf;

internal static class Textures
{
    private static byte[]? PngFromRgba(Texture.ImageBuffer image, byte[] data)
    {
        var width = (int)image.Width;
        var height = (int)image.Height;
        
        using var img = Image.LoadPixelData<Bgra32>(data, width, height);
        if (AllWhite(img, width, height))
        {
            return null;
        }

        var memoryStream = new MemoryStream();
        img.SaveAsPng(memoryStream);
        return memoryStream.GetBuffer();
    }

    private static bool AllWhite(Image<Bgra32> img, int width, int height)
    {
        Bgra32 white = Color.White;
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var pixel = img[x, y];
                if (pixel != white)
                {
                    return false;
                }
            }
        }

        return true;
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
                    var rgba = tex.GetSubimageAsRGBA(i);
                    var memoryPng = PngFromRgba(subImage.Value, rgba);

                    if (memoryPng != null)
                    {
                        var material = new MaterialBuilder()
                            .WithDoubleSide(true)
                            .WithAlpha(AlphaMode.MASK)
                            .WithBaseColor(memoryPng);

                        texturesByGuid[subImage.Value.UID] = material;
                    }
                }
            }
        }

        return texturesByGuid;
    }
}