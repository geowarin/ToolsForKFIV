using GltfKFIV.Gltf;
using ResourceKFIV.Asset;

namespace ToolsForKFIV.Actions;

public static class Export
{
    public static void ExportAsset(AssetType asset, string exportDir)
    {
        switch (asset)
        {
            case SceneAsset sceneAsset:
            {
                new GltfExporter(sceneAsset).Export(exportDir);
                break;
            }
            case ModelAsset modelAsset:
            {
                break;
            }
        }
    }
}