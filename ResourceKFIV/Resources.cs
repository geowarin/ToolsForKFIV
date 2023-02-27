using ResourceKFIV.Asset;
using ResourceKFIV.Filesystem;

namespace ResourceKFIV;

public class Resources
{
    private readonly IEnumerable<IResource> _resources;

    public Resources()
    {
        _resources = ResourceLoader.OpenKFivFile("/home/geo/Documents/King's Field - The Ancient City/SLUS_203.18");
    }
    
    public IEnumerable<IResource> GetMapsResources() =>
        _resources
            .Where(resource => resource.RelativePath.EndsWith(".map"));

    public IEnumerable<SceneAsset> GetMaps() =>
        GetMapsResources()
            .Select(ResourceManager.LoadAsset)
            .OfType<SceneAsset>();
}