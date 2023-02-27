namespace ResourceKFIV.Filesystem;

public class SystemResource : IResource
{
    public string RelativePath { get; }

    private string Path { get; }

    public SystemResource(string virtualPath, string osPath)
    {
        RelativePath = virtualPath;
        Path = osPath;
    }

    public byte[] Buffer => GetBuffer();

    private byte[] GetBuffer()
    {
        if (!File.Exists(Path))
        {
            throw new Exception($"Bad File Path: {Path}!!");
        }

        using var reader = new BinaryReader(File.OpenRead(Path));
        return reader.ReadBytes((int)reader.BaseStream.Length);
    }
}