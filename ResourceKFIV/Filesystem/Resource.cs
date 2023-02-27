namespace ResourceKFIV.Filesystem;

public interface Resource
{
    string RelativePath { get; }

    byte[] Buffer { get; }
}