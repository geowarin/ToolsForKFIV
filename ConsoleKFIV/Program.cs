using ResourceKFIV;
using ToolsForKFIV.Actions;

// var map = new Resources().GetMaps()
//      .First(r => r.RelativePath == "DATA/KF4.DAT/000.map");
// Export.ExportAsset(map, "/home/geo/Documents/maps");

foreach (var map in new Resources().GetMaps())
{
    Console.WriteLine($"==== {map.RelativePath} ====");
    Export.ExportAsset(map, "/home/geo/Documents/maps");
}
// View.ViewAsset(map);