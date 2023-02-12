using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using FormatKFIV.Asset;
using FormatKFIV.FileFormat;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Scenes;
using SharpGLTF.Transforms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ToolsForKFIV.Utility;

namespace ToolsForKFIV.Gltf;

public class GltfExporter
{
    private readonly Scene _sceneData;

    public GltfExporter(Scene sceneData)
    {
        _sceneData = sceneData;
    }

    public void Export(string totoGlb)
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
                        .WithChannelImage(KnownChannel.BaseColor, memoryPng);

                    texturesByGuid[subImage.Value.UID] = material;
                }
            }
        }

        var gltf = new SceneBuilder();

        foreach (Scene.Chunk chunk in scene.chunks)
        {
            var sceneBuilder = new SceneBuilder();
            //Construct draw geometry
            if (chunk.drawModelID >= 0)
            {
                var model = scene.omdData[chunk.drawModelID];
                var position = new Vector3(chunk.position.X, chunk.position.Y, chunk.position.Z);
                var rotation = new Vector3(chunk.rotation.X, chunk.rotation.Y, chunk.rotation.Z);
                var scale = new Vector3(chunk.scale.X, chunk.scale.Y, chunk.scale.Z);

                Console.Out.WriteLine($"position={position},rotation={rotation},scale = {scale}");
                // Matrix4x4Factory.CreateFrom()
                // var Name = $"GeoDrawChunk ({chunk.drawModelID.ToString("D4")})";


                foreach (Model.Mesh mesh in model.Meshes)
                {
                    var meshBuilder = MeshBuilder(model, mesh, texturesByGuid);
                    gltf.AddRigidMesh(meshBuilder, Matrix4x4.Identity);
                }

                gltf.AddScene(sceneBuilder, Matrix4x4.Identity);
            }
        }

        var gltfModel = gltf.ToGltf2();
        // gltfModel.SaveGLTF("mesh.gltf");
        Directory.CreateDirectory("export");
        gltfModel.SaveGLB("export/map.glb");
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
            if (meshPrimitive is Model.TrianglePrimitive p)
            {
                var vertexBuilders =
                    Enumerable.Range(0, 3)
                        .Select(i =>
                        {
                            var vertex = model.Vertices[p.Indices[0 + i]];
                            var normal = model.Normals[p.Indices[3 + i]];
                            var uv = model.Texcoords[p.Indices[6 + i]];
                            var color = model.Colours[p.Indices[9 + i]];

                            var vertexBuilder =
                                new VertexBuilder<VertexPositionNormal, VertexColor1Texture1, VertexEmpty>();

                            vertexBuilder.Geometry = new VertexPositionNormal(
                                vertex.X, vertex.Y, vertex.Z,
                                normal.X, normal.Y, normal.Z
                            );
                            vertexBuilder.Material = new VertexColor1Texture1(
                                new Vector4(color.R, color.G, color.B, color.A),
                                new Vector2(uv.U, uv.V)
                            );
                            return vertexBuilder;
                        }).ToArray();

                var prim = meshBuilder.UsePrimitive(material);
                prim.AddTriangle(
                    vertexBuilders[0],
                    vertexBuilders[1],
                    vertexBuilders[2]
                );
            }
        }

        return meshBuilder;
    }
}