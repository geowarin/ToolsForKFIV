using FormatKFIV.Asset;

namespace ToolsForKFIV.Asset;

public record AssetType;

public record ModelAsset(Model Model, Texture Texture) : AssetType;
record ParamAsset(Param Param) : AssetType;
record SceneAsset(Scene Scene) : AssetType;
record TextureAsset(Texture Texture) : AssetType;