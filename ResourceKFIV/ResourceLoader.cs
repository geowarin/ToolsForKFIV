using FormatKFIV.FileFormat;
using ResourceKFIV.Filesystem;

namespace ResourceKFIV;

public abstract class ResourceLoader
{
    public static IEnumerable<IResource> OpenKFivFile(string path)
    {
        var foundPath = Path.GetDirectoryName(path);
        if (foundPath == null)
        {
            throw new Exception($"Invalid path {path}");
        }
        
        foreach (var dir in Directory.GetDirectories(foundPath, "*", SearchOption.AllDirectories))
        {
            foreach (var file in Directory.GetFiles(dir))
            {
                var vfsPath = Path.GetRelativePath(foundPath, file);
                if (!vfsPath.Contains("KF4.DAT"))
                {
                    yield return new SystemResource(vfsPath, file);
                }
            }
        }

        foreach (var file in Directory.GetFiles(foundPath))
        {
            var vfsPath = Path.GetRelativePath(foundPath, file);;
            yield return new SystemResource(vfsPath, file);
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
            var dataDat = FFResourceDAT.LoadFromFile(Path.Combine(foundPath, datFile));
            for (var i = 0; i < dataDat.FileCount; ++i)
            {
                var vfsPath = Path.Combine(datFile, dataDat[i].name);
                yield return new VirtualResource(vfsPath, dataDat[i].buffer);
            }
        }
    }
}