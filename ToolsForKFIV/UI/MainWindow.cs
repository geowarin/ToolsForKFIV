using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FormatKFIV.Asset;
using FormatKFIV.FileFormat;
using ToolsForKFIV.Asset;
using ToolsForKFIV.Gltf;
using ToolsForKFIV.UI.Control;

namespace ToolsForKFIV;

public class MainWindow
{
    public static IEnumerable<Resource> OpenKFivFile(string path)
    {
        string foundFile = path;
        string foundPath = Path.GetDirectoryName(foundFile);

        ResourceManager.vfs.Reset();
        ResourceManager.vfs.SetRoot(foundPath + Path.DirectorySeparatorChar);

        //Scan OS File System
        foreach (string dir in Directory.GetDirectories(foundPath, "*", SearchOption.AllDirectories))
        {
            foreach (string file in Directory.GetFiles(dir))
            {
                string vfsPath = file.Replace(foundPath + Path.DirectorySeparatorChar, "");

                if (!vfsPath.Contains("KF4.DAT"))
                {
                    ResourceManager.vfs.PutResource(new SystemResource(vfsPath, file));
                }
            }
        }

        foreach (string file in Directory.GetFiles(foundPath))
        {
            string vfsPath = file.Replace(foundPath + Path.DirectorySeparatorChar, "");
            ResourceManager.vfs.PutResource(new SystemResource(vfsPath, file));
        }

        string[] kf4DatFiles;
        string kf4Region = "None";

        switch (Path.GetFileName(foundFile))
        {
            case "SLUS_203.18":
            case "SLUS_203.53":
                kf4Region = "NTSC";
                kf4DatFiles = new string[]
                {
                    $"DATA{Path.DirectorySeparatorChar}KF4.DAT",
                };
                break;

            case "SLPS_250.57":
                kf4Region = "NTSC-J";
                kf4DatFiles = new string[]
                {
                    $"DATA{Path.DirectorySeparatorChar}KF4.DAT",
                };
                break;

            case "SLES_509.20":
                kf4Region = "PAL";
                kf4DatFiles = new string[]
                {
                    $"DATA{Path.DirectorySeparatorChar}KF4_ENG.DAT",
                };
                break;

            default:
                kf4DatFiles = new string[] { "None" };
                break;
        }

        if (kf4Region == "None")
        {
            Logger.LogError("Invalid KF4 Data! Did you try to trick the system by renaming something?");
            Logger.LogError($"{foundFile}");
            return Array.Empty<Resource>();
        }

        Logger.LogInfo($"Loading KFIV Data (region: {kf4Region}, exe: {foundFile})");

        //Scan KF File System
        FFResourceDAT dataDat;
        foreach (string datFile in kf4DatFiles)
        {
            dataDat = FFResourceDAT.LoadFromFile(foundPath + Path.DirectorySeparatorChar + datFile);
            for (int i = 0; i < dataDat.FileCount; ++i)
            {
                string vfsPath = datFile + Path.DirectorySeparatorChar +
                                 dataDat[i].name.Replace('/', Path.DirectorySeparatorChar);
                ResourceManager.vfs.PutResource(new VirtualResource(vfsPath, dataDat[i].buffer));
            }
        }

        return ResourceManager.vfs.GetResources();
    }

    private static byte[] GetBuffer(Resource resource)
    {
        Type resourceType = resource.GetType();
        switch (resource)
        {
            case VirtualResource vres:
                Logger.LogInfo("Opened file is VIRTUAL type!");
                return vres.Buffer;

            case SystemResource sres:
                byte[] fileBuffer;
                Logger.LogInfo("Opened file is SYSTEM type!");

                if (!sres.GetBuffer(out fileBuffer))
                {
                    throw new Exception("Failed to aquire system file buffer!!!");
                }

                return fileBuffer;

            default:
                throw new Exception($"An invalid object is inside the VFS ({resourceType.Name}), WTF?");
        }
    }

    public static AssetType OpenResource(Resource resource)
    {
        var fileExtension = Path.GetExtension(resource.RelativePath);
        var fileBuffer = GetBuffer(resource);

        if (!ResourceManager.FormatIsSupported(fileExtension, fileBuffer, out var formatType, out var formatHandler))
        {
            throw new Exception($"Unable to find format handler for file! (file: {resource.RelativePath})");
        }

        switch (formatType)
        {
            default:
            case FEType.None:
                throw new Exception("Invalid file type!");

            case FEType.Param:
                Logger.LogInfo("Attempting to import parameters...");

                FIFormat<Param> paramHandler = (FIFormat<Param>)formatHandler;
                return new ParamAsset(paramHandler.LoadFromMemory(fileBuffer, out _, out _, out _));

            case FEType.Scene:
                Logger.LogInfo("Attempting to import scene...");

                FIFormat<Scene> sceneHandler = (FIFormat<Scene>)formatHandler;
                return new SceneAsset(sceneHandler.LoadFromMemory(fileBuffer, out _, out _, out _));

            case FEType.Texture:
                Logger.LogInfo("Attempting to import texture...");

                FIFormat<Texture> textureHandler = (FIFormat<Texture>)formatHandler;
                return new TextureAsset(textureHandler.LoadFromMemory(fileBuffer, out _, out _, out _));

            case FEType.Model:
                Logger.LogInfo("Attempting to import model...");
                FIFormat<Model> modelHandler = (FIFormat<Model>)formatHandler;

                return new ModelAsset(
                    modelHandler.LoadFromMemory(fileBuffer, out _, out _, out _),
                    (Texture)modelHandler
                );
        }
    }
}