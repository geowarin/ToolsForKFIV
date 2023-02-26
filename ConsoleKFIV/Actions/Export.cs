using GltfKFIV.Gltf;
using ResourceKFIV.Asset;

namespace ToolsForKFIV.Actions;

public class Export
{
    public static void ExportAsset(AssetType asset)
    {
        switch (asset)
        {
            case SceneAsset sceneAsset:
            {
                var fileName = Path.GetFileName(asset.RelativePath);
                new GltfExporter(sceneAsset.Scene).Export(fileName);
                break;
            }
            case ModelAsset modelAsset:
            {
                break;
            }
        }
    }
}