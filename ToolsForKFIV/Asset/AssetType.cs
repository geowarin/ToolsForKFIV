using FormatKFIV.Asset;

namespace ToolsForKFIV.Asset;

public record AssetType(string RelativePath);

public record ModelAsset(string RelativePath, Model Model, Texture Texture) : AssetType(RelativePath);
record ParamAsset(string RelativePath, Param Param) : AssetType(RelativePath);
record SceneAsset(string RelativePath, Scene Scene) : AssetType(RelativePath);
record TextureAsset(string RelativePath, Texture Texture) : AssetType(RelativePath);