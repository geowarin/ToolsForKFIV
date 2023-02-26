using OpenGlKFIV.UI.Control;
using ToolsForKFIV.Asset;

namespace ToolsForKFIV.Actions;

public class View
{
    public static void ViewAsset(AssetType asset)
    {
        switch (asset)
        {
            case SceneAsset sceneAsset:
            {
                // var fileName = Path.GetFileName(resource.RelativePath);
                // new GltfExporter(sceneData).Export(fileName);
                using var scene = new ToolFFScene(800, 600, "Scene", sceneAsset.Scene);
                scene.Run();
                break;
            }
            case ModelAsset modelAsset:
            {
                using var model = new ToolFFModel(800, 600, "Model", modelAsset);
                model.Run();
                break;
            }
        }
    }
}