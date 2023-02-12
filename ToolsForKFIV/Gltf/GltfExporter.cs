using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using FormatKFIV.Asset;
using FormatKFIV.Utility;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;
using Scene = FormatKFIV.Asset.Scene;
using Texture = FormatKFIV.Asset.Texture;

namespace ToolsForKFIV.Gltf;

public class GltfExporter
{
    private readonly Scene _sceneData;

    public GltfExporter(Scene sceneData)
    {
        _sceneData = sceneData;
    }

    public void Export(string fileName)
    {
        var scene = _sceneData;

        var texturesByGuid = new Dictionary<uint, MaterialBuilder>();
        foreach (Texture tex in scene.texData)
        {
            for (int i = 0; i < tex.SubimageCount; ++i)
            {
                Texture.ImageBuffer? subImage = tex.GetSubimage(i);
                if (subImage != null)
                {
                    var rgba = tex.GetSubimageAsRGBA(i);
                    var memoryPng = PngFromRgba(subImage.Value, rgba);

                    var material = new MaterialBuilder()
                        .WithDoubleSide(true)
                        .WithChannelImage(KnownChannel.BaseColor, memoryPng);

                    texturesByGuid[subImage.Value.UID] = material;
                }
            }
        }

        var gltf = new SceneBuilder();

        foreach (var chunk in scene.chunks)
        {
            if (chunk.drawModelID >= 0)
            {
                var model = scene.omdData[chunk.drawModelID];
                var trans = (chunk.position, chunk.rotation, chunk.scale);
                var sceneBuilder = MakeScene(trans, model, texturesByGuid);
                gltf.AddScene(sceneBuilder, Matrix4x4.Identity);
            }
        }

        var lights = new List<(AffineTransform transform, float radius, Vector3 color)>();  
        foreach (var obj in scene.objects)
        {
            var trans = (obj.position, obj.rotation, obj.scale);
            
            switch (obj.classID)
            {
                case 0x01FB:
                case 0x01FC:
                    int radius = (obj.classParams[11] << 8) | obj.classParams[10];
                    
                    var (position, rotation, scale) = trans;
                    var quaternion = Quaternion.Identity;
                    var transform = new AffineTransform(scale.ToVector3(), quaternion, position.ToVector3());

                    var color = new Vector3(obj.classParams[4], obj.classParams[6], obj.classParams[8]);

                    lights.Add((transform, radius, color));
                    
                    break;

                //Until we find exactly what value in the object struct decides if the object uses a OM2 model, this is  the best way.
                case 0x001A:
                case 0x0020:
                case 0x0041:
                case 0x0044:
                case 0x0045:
                case 0x0046:
                    if (obj.drawModelID >= 0)
                    {
                        var model = scene.om2Data[obj.drawModelID];
                        MakeScene(trans, model, texturesByGuid);
                        var sceneBuilder = MakeScene(trans, model, texturesByGuid);
                        gltf.AddScene(sceneBuilder, Matrix4x4.Identity);
                    }

                    break;

                default:
                    if (obj.drawModelID >= 0)
                    {
                        var model = scene.omdData[obj.drawModelID];
                        MakeScene(trans, model, texturesByGuid);
                        var sceneBuilder = MakeScene(trans, model, texturesByGuid);
                        gltf.AddScene(sceneBuilder, Matrix4x4.Identity);
                    }

                    break;
            }
        }


        foreach (var item in scene.items)
        {
            var model = scene.omdData[item.omdID];
            var trans = (item.position, item.rotation, item.scale);

            var sceneBuilder = MakeScene(trans, model, texturesByGuid);
            gltf.AddScene(sceneBuilder, Matrix4x4.Identity);
        }

        var gltfModel = gltf.ToGltf2();

        // foreach (var light in lights)
        // {
        //     var punctualLight = gltfModel
        //         .CreatePunctualLight(PunctualLightType.Point)
        //         .WithColor(light.color, 1f, light.radius);
        //     
        //     gltfModel.LogicalPunctualLights
        //     gltfModel.AddLight(punctualLight, light.transform);
        //     // transform ????
        // }
        // gltfModel.SaveGLTF("mesh.gltf");
        Directory.CreateDirectory("export");
        gltfModel.SaveGLB($"export/{fileName}.glb");
    }

    private static SceneBuilder MakeScene((Vector3f position, Vector3f rotation, Vector3f scale) trans, Model model,
        Dictionary<uint, MaterialBuilder> texturesByGuid)
    {
        var (position, rotation, scale) = trans;
        var sceneBuilder = new SceneBuilder();
        // var quaternion = Quaternion.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z);
        var quaternion = Quaternion.Identity;
        var transform = new AffineTransform(scale.ToVector3(), quaternion, position.ToVector3());

        foreach (Model.Mesh mesh in model.Meshes)
        {
            var meshBuilder = MeshBuilder(model, mesh, texturesByGuid);
            sceneBuilder.AddRigidMesh(meshBuilder, transform);
        }

        return sceneBuilder;
    }

    private byte[] PngFromRgba(Texture.ImageBuffer image, byte[] data)
    {
        var width = (int)image.Width;
        var height = (int)image.Height;

        using var img = Image.LoadPixelData<Rgba32>(data, width, height);
        var memoryStream = new MemoryStream();
        img.SaveAsPng(memoryStream);
        return memoryStream.GetBuffer();
    }

    private static MeshBuilder<VertexPositionNormal, VertexColor1Texture1> MeshBuilder(Model model, Model.Mesh mesh,
        Dictionary<uint, MaterialBuilder> materials)
    {
        var material = new MaterialBuilder();
        if (mesh.textureSlot != -1)
        {
            var textureSlot = mesh.textureSlot;
            var textureUid = model.TextureSlots[textureSlot].slotKey;

            //Ensure only valid texture slots are rendered.
            if (model.TextureSlots[mesh.textureSlot].slotKey != 0x00000000 && materials.ContainsKey(textureUid))
            {
                material = materials[textureUid];
            }
        }

        var meshBuilder = new MeshBuilder<VertexPositionNormal, VertexColor1Texture1>("mesh");
        foreach (var meshPrimitive in mesh.primitives)
        {
            switch (meshPrimitive)
            {
                case Model.TrianglePrimitive mp:
                {
                    var vertexBuilders =
                        Enumerable.Range(0, 3)
                            .Select(i =>
                            {
                                var vertex = model.Vertices[mp.Indices[0 + i]];
                                var normal = model.Normals[mp.Indices[3 + i]];
                                var uv = model.Texcoords[mp.Indices[6 + i]];
                                var color = model.Colours[mp.Indices[9 + i]];

                                var vertexBuilder =
                                    new VertexBuilder<VertexPositionNormal, VertexColor1Texture1, VertexEmpty>
                                    {
                                        Geometry = new VertexPositionNormal(
                                            vertex.X, vertex.Y, vertex.Z,
                                            normal.X, normal.Y, normal.Z
                                        ),
                                        Material = new VertexColor1Texture1(
                                            new Vector4(color.R, color.G, color.B, color.A),
                                            new Vector2(uv.U, uv.V)
                                        )
                                    };

                                return vertexBuilder;
                            }).ToArray();

                    var prim = meshBuilder.UsePrimitive(material);
                    prim.AddTriangle(
                        vertexBuilders[0],
                        vertexBuilders[1],
                        vertexBuilders[2]
                    );
                    break;
                }
                case Model.LinePrimitive lp:
                    Console.Out.WriteLine("line");
                    break;
            }
        }

        return meshBuilder;
    }
}

static class Ext
{
    public static Vector3 ToVector3(this Vector3f v) => new(v.X, v.Y, v.Z);
}