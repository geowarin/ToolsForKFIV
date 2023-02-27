namespace ResourceKFIV.Filesystem;

public interface IResource
{
    string RelativePath { get; }

    byte[] Buffer { get; }
}