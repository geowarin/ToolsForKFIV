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
using SharpGLTF.Transforms;

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

        var texturesByGuid = GenerateTextures(scene);

        var gltf = new SceneBuilder(fileName);

        var numChunk = 0;
        foreach (var chunk in scene.chunks)
        {
            if (chunk.drawModelID >= 0)
            {
                var model = scene.omdData[chunk.drawModelID];
                var trans = MakeTransform((chunk.position, chunk.rotation, chunk.scale));
                var sceneBuilder = MakeScene($"Chunk-{numChunk++}", trans, model, texturesByGuid);
                gltf.AddScene(sceneBuilder, Matrix4x4.Identity);
            }
        }

        var numObj = 0;
        foreach (var obj in scene.objects)
        {
            var transform = MakeTransform((obj.position, obj.rotation, obj.scale));

            switch (obj.classID)
            {
                case 0x01FB:
                case 0x01FC:
                    var light = MakeLight("Light", obj);
                    gltf.AddLight(light, transform);
                    break;

                default:
                    if (obj.drawModelID >= 0)
                    {
                        var model = scene.omdData[obj.drawModelID];
                        var sceneBuilder = MakeScene($"Object-{numObj++}", transform, model, texturesByGuid);
                        gltf.AddScene(sceneBuilder, Matrix4x4.Identity);
                    }

                    break;
            }
        }

        foreach (var item in scene.items)
        {
            var model = scene.omdData[item.omdID];
            var transform = MakeTransform((item.position, item.rotation, item.scale));
            var sceneBuilder = MakeScene($"Item-{item.omdID}", transform, model, texturesByGuid);
            gltf.AddScene(sceneBuilder, Matrix4x4.Identity);
        }

        gltf.ApplyBasisTransform(Matrix4x4.CreateScale(0.01f));
        var gltfModel = gltf.ToGltf2();

        Directory.CreateDirectory("export");
        gltfModel.SaveGLB($"export/{fileName}.glb");
    }

    private static SceneBuilder MakeScene(string sceneName, AffineTransform transform, Model model,
        Dictionary<uint, MaterialBuilder> texturesByGuid)
    {
        var sceneBuilder = new SceneBuilder(sceneName);

        var meshNum = 0;
        foreach (var mesh in model.Meshes)
        {
            var meshBuilder = MakeMesh($"{sceneName}_Mesh-{meshNum++}", model, mesh, texturesByGuid);
            sceneBuilder.AddRigidMesh(meshBuilder, transform);
        }

        return sceneBuilder;
    }

    private static LightBuilder.Point MakeLight(string name, Scene.Object obj)
    {
        var radius = (obj.classParams[11] << 8) | obj.classParams[10];
        var color = new Vector3(obj.classParams[4], obj.classParams[6], obj.classParams[8]);
        var light = new LightBuilder.Point
        {
            Name = name,
            Range = radius,
            Intensity = radius,
            Color = color
        };
        return light;
    }

    private AffineTransform MakeTransform((Vector3f position, Vector3f rotation, Vector3f scale) trans)
    {
        // var quaternion = Quaternion.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z);
        var quaternion = Quaternion.Identity;
        var transform = new AffineTransform(trans.scale.ToVector3(), quaternion, trans.position.ToVector3());
        return transform;
    }


    private Dictionary<uint, MaterialBuilder> GenerateTextures(Scene scene)
    {
        var texturesByGuid = new Dictionary<uint, MaterialBuilder>();
        foreach (var tex in scene.texData)
        {
            for (var i = 0; i < tex.SubimageCount; ++i)
            {
                var subImage = tex.GetSubimage(i);
                if (subImage != null)
                {
                    var rgba = tex.GetSubimageAsRGBA(i);
                    var memoryPng = Textures.PngFromRgba(subImage.Value, rgba);

                    var material = new MaterialBuilder()
                        .WithDoubleSide(true)
                        .WithBaseColor(memoryPng);

                    texturesByGuid[subImage.Value.UID] = material;
                }
            }
        }

        return texturesByGuid;
    }

    private static MeshBuilder<VertexPositionNormal, VertexColor1Texture1> MakeMesh(string name, Model model,
        Model.Mesh mesh,
        IReadOnlyDictionary<uint, MaterialBuilder> materials)
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
            else
            {
                Console.Out.WriteLine($"${name} has no texure");
            }
        }

        var meshBuilder = new MeshBuilder<VertexPositionNormal, VertexColor1Texture1>(name);
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
                case Model.LinePrimitive:
                    throw new Exception("Does not handle line primitive");
            }
        }

        return meshBuilder;
    }
}