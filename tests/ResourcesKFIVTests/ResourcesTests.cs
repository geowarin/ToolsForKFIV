using System;
using System.Linq;
using NUnit.Framework;
using ResourceKFIV;
using ResourceKFIV.Asset;
using ResourceKFIV.Filesystem;

namespace ResourcesKFIVTests;

public class ResourcesTests
{
    [Test]
    public void list_resources()
    {
        var maps = new Resources().GetMaps();

        Assert.AreEqual(
            new[]
            {
                "DATA/KF4.DAT/000.map",
                "DATA/KF4.DAT/002.map",
                "DATA/KF4.DAT/004.map",
                "DATA/KF4.DAT/005.map",
                "DATA/KF4.DAT/006.map",
                "DATA/KF4.DAT/007.map",
                "DATA/KF4.DAT/008.map",
                "DATA/KF4.DAT/009.map",
                "DATA/KF4.DAT/011.map",
                "DATA/KF4.DAT/012.map",
                "DATA/KF4.DAT/013.map",
                "DATA/KF4.DAT/014.map",
                "DATA/KF4.DAT/015.map",
                "DATA/KF4.DAT/016.map",
                "DATA/KF4.DAT/017.map",
                "DATA/KF4.DAT/018.map",
                "DATA/KF4.DAT/019.map",
                "DATA/KF4.DAT/020.map",
                "DATA/KF4.DAT/021.map",
                "DATA/KF4.DAT/022.map",
                "DATA/KF4.DAT/023.map",
                "DATA/KF4.DAT/024.map",
                "DATA/KF4.DAT/025.map",
                "DATA/KF4.DAT/026.map",
                "DATA/KF4.DAT/027.map",
                "DATA/KF4.DAT/028.map",
                "DATA/KF4.DAT/029.map",
                "DATA/KF4.DAT/030.map",
                "DATA/KF4.DAT/031.map",
                "DATA/KF4.DAT/032.map",
                "DATA/KF4.DAT/033.map",
                "DATA/KF4.DAT/034.map",
                "DATA/KF4.DAT/035.map",
                "DATA/KF4.DAT/036.map",
                "DATA/KF4.DAT/037.map",
                "DATA/KF4.DAT/038.map",
                "DATA/KF4.DAT/039.map",
                "DATA/KF4.DAT/040.map",
                "DATA/KF4.DAT/041.map",
                "DATA/KF4.DAT/042.map",
                "DATA/KF4.DAT/043.map",
                "DATA/KF4.DAT/044.map",
                "DATA/KF4.DAT/095.map",
                "DATA/KF4.DAT/096.map",
                "DATA/KF4.DAT/097.map",
                "DATA/KF4.DAT/098.map",
                "DATA/KF4.DAT/099.map",
                "DATA/KF4.DAT/100.map",
                "DATA/KF4.DAT/101.map",
                "DATA/KF4.DAT/102.map",
                "DATA/KF4.DAT/103.map",
                "DATA/KF4.DAT/104.map",
                "DATA/KF4.DAT/105.map",
                "DATA/KF4.DAT/106.map",
                "DATA/KF4.DAT/107.map",
                "DATA/KF4.DAT/108.map",
                "DATA/KF4.DAT/109.map",
                "DATA/KF4.DAT/110.map",
                "DATA/KF4.DAT/111.map",
                "DATA/KF4.DAT/112.map",
                "DATA/KF4.DAT/113.map",
                "DATA/KF4.DAT/114.map",
                "DATA/KF4.DAT/115.map",
                "DATA/KF4.DAT/116.map"
            },
            maps.Select(resource => resource.RelativePath)
        );
    }

    [Test]
    public void open_resource()
    {
        var map = new Resources().GetMaps().First();
        var asset = ResourceLoader.OpenResource(map);
        
        if (asset is SceneAsset sceneAsset)
        {
            Assert.IsTrue(sceneAsset.Scene.chunks.Count > 0);
        }
        else
        {
            Assert.Fail("Should be a scene assset");
        }
    }
}
