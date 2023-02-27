using ResourceKFIV;
using ToolsForKFIV.Actions;

var map = new Resources().GetMaps()
    .First(r => r.RelativePath == "DATA/KF4.DAT/000.map");

// Export.ExportAsset(asset);
View.ViewAsset(map);