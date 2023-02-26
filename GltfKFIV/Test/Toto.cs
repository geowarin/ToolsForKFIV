using System.Numerics;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;

namespace GltfKFIV.Test;

public class Toto
{
    public static void LezGong()
    {
        // Works();
        DoesNotWork();

        // Caks();
    }

    private static void Caks()
    {
        var gltf = new SceneBuilder("toto");

        var red = new Vector4(1, 0, 0, 0);
        var blue = new Vector4(0, 0, 1, 0);

        var cube = new MeshBuilder<VertexPosition, VertexEmpty, VertexEmpty>("Cube");
        cube.AddCube(new MaterialBuilder().WithBaseColor(red), Matrix4x4.Identity);

        var quat = Quaternion.CreateFromYawPitchRoll(45, 0, 0);
        var cubeTransform1 = new AffineTransform(
            Vector3.One,
            quat,
            new Vector3(0, 0, 0)
        );
        gltf.AddRigidMesh(cube, cubeTransform1);


        // var cube2 = new MeshBuilder<VertexPosition, VertexEmpty, VertexEmpty>("Cube");
        // cube2.AddCube(new MaterialBuilder().WithBaseColor(blue), Matrix4x4.Identity);
        //
        // var cubeTransform2 = new AffineTransform(
        //     Vector3.One,
        //     Quaternion.Identity,
        //     new Vector3(1, 0, 0)
        // );
        // gltf.AddRigidMesh(cube2, cubeTransform2);


        var lightScene = new SceneBuilder();

        var spot = new LightBuilder.Point
        {
            Color = new Vector3(1, 0, 0),
            Intensity = 10,
            Range = 10,
        };
        var lightTransform = new AffineTransform(
            Vector3.One,
            Quaternion.Identity,
            new Vector3(0, 2, 0)
        );

        lightScene.AddLight(spot, lightTransform);
        gltf.AddScene(lightScene, Matrix4x4.Identity);

        var gltfModel = gltf.ToGltf2();


        Directory.CreateDirectory("export");
        // gltfModel.SaveGLB($"export/toto.glb");
        gltfModel.SaveGLTF($"export/toto.gltf");
    }

    public static void DoesNotWork()
    {
        var gltf = new SceneBuilder();
        var light = new LightBuilder.Point
        {
            Color = new Vector3(1, 0, 0),
            Intensity = 3,
            Range = 10,
        };
        gltf.AddLight(light, AffineTransform.Identity);
        gltf.ToGltf2().Save("export/testLight.gltf");
    }

    private static void Works()
    {
        var root = ModelRoot.CreateModel();
        var scene = root.UseScene("Empty Scene");

        scene.CreateNode()
            .PunctualLight = root.CreatePunctualLight(PunctualLightType.Point)
            .WithColor(Vector3.UnitY, 3, 10);

        root.SaveGLTF("export/root.gltf");
    }
}