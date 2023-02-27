using System.Diagnostics.CodeAnalysis;
using FormatKFIV.Asset;
using FormatKFIV.FileFormat;
using ResourceKFIV.Asset;
using ResourceKFIV.Filesystem;

namespace ResourceKFIV;

public static class ResourceManager
{
    private static readonly List<FIFormat<Model>> FormatsModel = new()
    {
        new FFModelICO(),
        new FFModelOMD(),
        new FFModelOM2(),
        new FFModelMOD(),
        new FFModelCHR(),
        new FFModelOBJ()
    };

    private static readonly List<FIFormat<Texture>> FormatsTexture = new()
    {
        new FFTextureTX2(),
        new FFTextureTM2(),
        new FFTextureTMX(),
        new FFTextureTGA(),
        new FFTexturePNG()
    };

    private static readonly List<FIFormat<Scene>> FormatsScene = new()
    {
        new FFSceneMAP()
    };

    private static readonly List<FIFormat<Param>> FormatsParam = new()
    {
        new FFParamReverb(),
        new FFParamItemName(),
        new FFParamWeapon(),
        new FFParamMagic()
    };


    [SuppressMessage("ReSharper", "SpecifyStringComparison")]
    private static bool CanHandle<T>(FIFormat<T> fmt, byte[] fileBuffer, string fileExt)
    {
        return fmt.Parameters.Extensions
            .Where(extension => extension.ToLower() == fileExt.ToLower())
            .Any(_ => fmt.Parameters.Validator(fileBuffer));
    }

    public static AssetType LoadAsset(IResource resource)
    {
        var relativePath = resource.RelativePath;
        var fileExt = Path.GetExtension(relativePath);
        var fileBuffer = resource.Buffer;

        var modelFormat = FormatsModel.FirstOrDefault(format => CanHandle(format, fileBuffer, fileExt));
        if (modelFormat != null)
        {
            return new ModelAsset(
                relativePath,
                modelFormat.LoadFromMemory(fileBuffer, out var modelTextureData, out _, out _),
                (Texture)modelTextureData
            );
        }

        var textureFormat = FormatsTexture.FirstOrDefault(format => CanHandle(format, fileBuffer, fileExt));
        if (textureFormat != null)
        {
            return new TextureAsset(relativePath, textureFormat.LoadFromMemory(fileBuffer, out _, out _, out _));
        }

        var sceneFormat = FormatsScene.FirstOrDefault(format => CanHandle(format, fileBuffer, fileExt));
        if (sceneFormat != null)
        {
            return new SceneAsset(relativePath, sceneFormat.LoadFromMemory(fileBuffer, out _, out _, out _));
        }

        var paramsFormat = FormatsParam.FirstOrDefault(format => CanHandle(format, fileBuffer, fileExt));
        if (paramsFormat != null)
        {
            return new ParamAsset(relativePath, paramsFormat.LoadFromMemory(fileBuffer, out _, out _, out _));
        }


        throw new Exception($"No handler found for resource {resource.RelativePath}");
    }
}