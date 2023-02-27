using FormatKFIV.Asset;
using FormatKFIV.FileFormat;
using ResourceKFIV.Asset;

namespace ResourceKFIV.Filesystem;

public static class ResourceManager
{
    private static List<FIFormat<Model>> formatsModel;
    private static List<FIFormat<Texture>> formatsTexture;
    private static List<FIFormat<Scene>> formatsScene;
    private static List<FIFormat<Param>> formatsParam;

    public static void Initialize()
    {
        //Initialize Model Handlers
        formatsModel = new List<FIFormat<Model>>
        {
            new FFModelICO(),
            new FFModelOMD(),
            new FFModelOM2(),
            new FFModelMOD(),
            new FFModelCHR(),
            new FFModelOBJ()
        };

        //Initialize Texture Handlers
        formatsTexture = new List<FIFormat<Texture>>
        {
            new FFTextureTX2(),
            new FFTextureTM2(),
            new FFTextureTMX(),
            new FFTextureTGA(),
            new FFTexturePNG()
        };

        //Initialize Scene Handlers
        formatsScene = new List<FIFormat<Scene>>
        {
            new FFSceneMAP()
        };

        //Initialize Param Handlers
        formatsParam = new List<FIFormat<Param>>
        {
            new FFParamReverb(),
            new FFParamItemName(),
            new FFParamWeapon(),
            new FFParamMagic()
        };
    }

    public static AssetType GetHandler(Resource resource)
    {
        var relativePath = resource.RelativePath;
        var fileExt = Path.GetExtension(relativePath);
        var fileBuffer = resource.Buffer;

        foreach (FIFormat<Model> fmt in formatsModel)
        {
            foreach (var extension in fmt.Parameters.Extensions)
            {
                if (extension.ToLower() != fileExt.ToLower())
                {
                    continue;
                }

                if (!fmt.Parameters.Validator(fileBuffer))
                {
                    continue;
                }
                
                return new ModelAsset(
                    relativePath, 
                    fmt.LoadFromMemory(fileBuffer, out var modelTextureData, out _, out _),
                    (Texture)modelTextureData
                );
            }
        }

        foreach (FIFormat<Texture> fmt in formatsTexture)
        {
            foreach (var extension in fmt.Parameters.Extensions)
            {
                if (extension.ToLower() != fileExt.ToLower())
                {
                    continue;
                }

                if (!fmt.Parameters.Validator(fileBuffer))
                {
                    continue;
                }

                return new TextureAsset(relativePath, fmt.LoadFromMemory(fileBuffer, out _, out _, out _));
            }
        }

        foreach (FIFormat<Scene> fmt in formatsScene)
        {
            foreach (var ext in fmt.Parameters.Extensions)
            {
                if (ext.ToLower() != fileExt.ToLower())
                {
                    continue;
                }

                if (!fmt.Parameters.Validator(fileBuffer))
                {
                    continue;
                }

                return new SceneAsset(relativePath, fmt.LoadFromMemory(fileBuffer, out _, out _, out _));
            }
        }

        foreach (FIFormat<Param> fmt in formatsParam)
        {
            foreach (var ext in fmt.Parameters.Extensions)
            {
                if (ext.ToLower() != fileExt.ToLower())
                {
                    continue;
                }

                if (!fmt.Parameters.Validator(fileBuffer))
                {
                    continue;
                }

                return new ParamAsset(relativePath, fmt.LoadFromMemory(fileBuffer, out _, out _, out _));
            }
        }

        throw new Exception($"No handler found for resource {resource.RelativePath}");
    }

    public static FIFormat<Texture>[] GetExportableTextureFormats()
    {
        var formats = new List<FIFormat<Texture>>();

        //Scan for exportable texture formats
        foreach (FIFormat<Texture> fmt in formatsTexture)
        {
            if (!fmt.Parameters.AllowExport)
                continue;
            formats.Add(fmt);
        }

        //Conversion to array and cleanup
        FIFormat<Texture>[] formatsArray = formats.ToArray();
        formats.Clear();

        return formatsArray;
    }

    public static FIFormat<Model>[] GetExportableModelFormats()
    {
        List<FIFormat<Model>> formats = new List<FIFormat<Model>>();

        //Scan for exportable texture formats
        foreach (FIFormat<Model> fmt in formatsModel)
        {
            if (!fmt.Parameters.AllowExport)
                continue;
            formats.Add(fmt);
        }

        //Conversion to array and cleanup
        FIFormat<Model>[] formatsArray = formats.ToArray();
        formats.Clear();

        return formatsArray;
    }
}