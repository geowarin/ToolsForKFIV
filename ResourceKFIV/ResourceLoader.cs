using FormatKFIV.Asset;
using FormatKFIV.FileFormat;
using ResourceKFIV.Asset;
using ResourceKFIV.Filesystem;

namespace ResourceKFIV;

public class ResourceLoader
{
    public static IEnumerable<Resource> OpenKFivFile(string path)
    {
        var foundPath = Path.GetDirectoryName(path);

        var vfs = new VirtualFileSystem();

        foreach (var dir in Directory.GetDirectories(foundPath, "*", SearchOption.AllDirectories))
        {
            foreach (var file in Directory.GetFiles(dir))
            {
                var vfsPath = file.Replace(foundPath + Path.DirectorySeparatorChar, "");

                if (!vfsPath.Contains("KF4.DAT"))
                {
                    vfs.PutResource(new SystemResource(vfsPath, file));
                }
            }
        }

        foreach (var file in Directory.GetFiles(foundPath))
        {
            var vfsPath = file.Replace(foundPath + Path.DirectorySeparatorChar, "");
            vfs.PutResource(new SystemResource(vfsPath, file));
        }

        string[] kf4DatFiles;
        var kf4Region = "None";

        switch (Path.GetFileName(path))
        {
            case "SLUS_203.18":
            case "SLUS_203.53":
                kf4Region = "NTSC";
                kf4DatFiles = new[]
                {
                    $"DATA{Path.DirectorySeparatorChar}KF4.DAT",
                };
                break;

            case "SLPS_250.57":
                kf4Region = "NTSC-J";
                kf4DatFiles = new[]
                {
                    $"DATA{Path.DirectorySeparatorChar}KF4.DAT",
                };
                break;

            case "SLES_509.20":
                kf4Region = "PAL";
                kf4DatFiles = new[]
                {
                    $"DATA{Path.DirectorySeparatorChar}KF4_ENG.DAT",
                };
                break;

            default:
                kf4DatFiles = new[] { "None" };
                break;
        }

        if (kf4Region == "None")
        {
            throw new Exception("Invalid KF4 Data! Did you try to trick the system by renaming something?");
        }

        foreach (var datFile in kf4DatFiles)
        {
            var dataDat = FFResourceDAT.LoadFromFile(foundPath + Path.DirectorySeparatorChar + datFile);
            for (var i = 0; i < dataDat.FileCount; ++i)
            {
                var vfsPath = datFile + Path.DirectorySeparatorChar +
                              dataDat[i].name.Replace('/', Path.DirectorySeparatorChar);
                vfs.PutResource(new VirtualResource(vfsPath, dataDat[i].buffer));
            }
        }

        return vfs.GetResources();
    }

    public static AssetType OpenResource(Resource resource)
    {
        return ResourceManager.GetHandler(resource);
    }
}