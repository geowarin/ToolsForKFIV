using System.Security.Cryptography;
using FormatKFIV.Asset;
using SharpGLTF.Materials;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GltfKFIV.Gltf;

internal static class Textures
{
     private static readonly MD5 HasherMD5 = MD5.Create();
    
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
    
    private static bool WriteRgba(Texture.ImageBuffer image, byte[] data, string path)
    {
        var width = (int)image.Width;
        var height = (int)image.Height;
        
        using var img = Image.LoadPixelData<Bgra32>(data, width, height);
        if (AllWhite(img, width, height))
        {
            return false;
        }

        img.SaveAsPng(path);
        return true;
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

    public static Dictionary<uint, MaterialBuilder> GenerateTextures(Scene scene, string exportPath)
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
                    // var memoryPng = PngFromRgba(subImage.Value, rgba);
                    
                    var path = Path.Combine(exportPath, Md5(rgba));
                    var validTexture = WriteRgba(subImage.Value, rgba, path);

                    if (validTexture)
                    {
                        var material = new MaterialBuilder()
                            .WithDoubleSide(true)
                            .WithAlpha(AlphaMode.MASK)
                            .WithBaseColor(path);

                        texturesByGuid[subImage.Value.UID] = material;
                    }
                }
            }
        }

        return texturesByGuid;
    }

    private static string Md5(byte[] rgba)
    {
        var hashArr = HasherMD5.ComputeHash(rgba);
        return Convert.ToHexString(hashArr);
    }
}