namespace ResourceKFIV.Filesystem
{
    public class VirtualFileSystem
    {
        private List<Resource> resources;
        private string osRoot = "";

        public VirtualFileSystem()
        {
            resources = new List<Resource>();
        }

        public Resource GetResource(int index, out Type type)
        {
            if(index < 0 || index > resources.Count)
            {
                Logger.LogWarn($"Resource index is out of bounds: {index}");
                type = null;
                return null;
            }

            Resource resource = resources[index];
            type = resource.GetType();

            return resource;
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

        public void SetRoot(string rootpath)
        {
            osRoot = rootpath;

            Logger.LogInfo($"VFS -> New OS Root = {rootpath}");
        }

        public void Reset()
        {
            resources.Clear();
            osRoot = "";
        }
    }
}
