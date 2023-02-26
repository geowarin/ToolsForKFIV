using ResourceKFIV.Filesystem;

namespace ResourceKFIV;

public class Resources
{
    private IEnumerable<Resource>? _resources;

    public Resources()
    {
        ResourceManager.Initialize();
        _resources = ResourceLoader.OpenKFivFile("/home/geo/Documents/King's Field - The Ancient City/SLUS_203.18");
    }
    
    public IEnumerable<Resource> GetMaps()
    {
        return _resources
            .Where(resource => resource.RelativePath.EndsWith(".map"));
    }
}