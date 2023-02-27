namespace ResourceKFIV.Filesystem;

public class VirtualResource : Resource
{
    public string RelativePath { get; }
    public byte[] Buffer { get; }

    //Constructor
    public VirtualResource(string path, byte[] buffer)
    {
        RelativePath = path;
        Buffer = buffer;
    }
}