namespace ResourceKFIV.Filesystem;

public class VirtualFileSystem
{
    private List<Resource> resources;

    public VirtualFileSystem()
    {
        resources = new List<Resource>();
    }

    public Resource[] GetResources()
    {
        return resources.ToArray();
    }

    /// <summary>Puts a resource into the virtual file system</summary>
    /// <param name="resource">A virtual or system resource</param>
    /// <returns>Returns true on success, false otherwise</returns>
    public void PutResource(Resource resource)
    {
        resources.Add(resource);
    }

}