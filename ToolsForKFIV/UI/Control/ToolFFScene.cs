using System;
using FormatKFIV.Asset;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ToolsForKFIV.Rendering;
using ToolsForKFIV.Utility;
using YamlDotNet.Core.Tokens;

namespace ToolsForKFIV.UI.Control;

public class ToolFFScene : GameWindow
{
    //Data
    private GLShader[] glShaders;
    private GLModel[] glSceneModels;
    private GLTexture[] glSceneTextures;

    private Vector3 cameraTo = new Vector3(0, 0, 0), cameraFrom = new Vector3(8, 4, 0);
    private float camLookX = 0, camLookY = 0;

    private Vector2 mousePosition = new Vector2(0, 0), lastMousePosition = new Vector2(0, 0);

    private Matrix4 matView, matProjection;

    Scene currentScene;
    IScene scene;

    public ToolFFScene(int width, int height, string title, Scene sceneData) :
        base(GameWindowSettings.Default, new NativeWindowSettings { Size = (width, height), Title = title })
    {
        SetSceneData(sceneData);
    }

    public void SetSceneData(Scene newScene)
    {
        if (scene != null)
        {
            //Clear Texture Data
            foreach (uint texKey in ResourceManager.glTextures.Keys)
            {
                GLTexture tex = ResourceManager.glTextures[texKey];
                tex.Destroy();
            }

            ResourceManager.glTextures.Clear();
            ResourceManager.glTextures.Add(0xDEADBEEF, GLTexture.Generate44Grid());

            scene.Dispose();
            scene = null;
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();

        currentScene = newScene;
        scene = new Scene3D(currentScene);

        //Populate Scene Node View
        // stSceneNodeTree.Nodes.Clear();
        //
        // foreach(ISceneNode sceneNode in scene.Nodes)
        // {
        //     RecursiveTreeViewNodes(null, sceneNode);
        // }
    }

    // private void RecursiveTreeViewNodes(TreeNode treeNode, ISceneNode sceneNode)
    // {
    //     TreeNode node = null;
    //     switch (sceneNode)
    //     {
    //         case SceneNodeCollection snc:
    //
    //             if (treeNode == null)
    //             {
    //                 node = stSceneNodeTree.Nodes.Add(sceneNode.Name, sceneNode.Name);
    //                 node.Tag = sceneNode;
    //             }
    //             else
    //             {
    //                 node =  treeNode.Nodes.Add(sceneNode.Name, sceneNode.Name);
    //                 node.Tag = sceneNode;
    //             }
    //
    //             foreach (ISceneNode childNode in snc.Children)
    //             {
    //                 RecursiveTreeViewNodes(node, childNode);
    //             }
    //
    //             break;
    //
    //         default:
    //             if (treeNode == null)
    //             {
    //                 node = stSceneNodeTree.Nodes.Add(sceneNode.Name, sceneNode.Name);
    //                 node.Tag = sceneNode;
    //             }
    //             else
    //             {
    //                 node = treeNode.Nodes.Add(sceneNode.Name, sceneNode.Name);
    //                 node.Tag = sceneNode;
    //             }
    //             break;
    //     }
    // }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        //Initialize Shaders
        glShaders = new GLShader[2];
        glShaders[0] = new GLShader(ResourceManager.ProgramDirectory + "Resource/shd_3DColour.vs",
            ResourceManager.ProgramDirectory + "Resource/shd_3DColour.fs");
        glShaders[1] = new GLShader(ResourceManager.ProgramDirectory + "Resource/shd_3DNormTexColour.vs",
            ResourceManager.ProgramDirectory + "Resource/shd_3DNormTexColour.fs");
        glShaders[1].SetUniformInt1("sDiffuse", 0);

        //Initialize Scene Viewer Models
        glSceneModels = new GLModel[3];
        glSceneModels[0] = GLModel.Generate3DGrid();

        //Initialize Scene Viewer Textures
        glSceneTextures = new GLTexture[2];
        glSceneTextures[0] = GLTexture.Generate44Grid();
        glSceneTextures[1] = GLTexture.Generate44White();

        ResourceManager.glTextures.Add(0xDEADBEEF, GLTexture.Generate44Grid());

        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
        matProjection = Matrix4.CreatePerspectiveFieldOfView(0.7f, e.Width / (float)e.Height, 0.1f, 65536f);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        KeyboardState input = KeyboardState;
        if (input.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        HandleInputs();

        Paint();
    }

    private void Paint()
    {
        GL.ClearColor(ResourceManager.settings.mtBgCC.ToColor());
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.Enable(EnableCap.Multisample);
        GL.Enable(EnableCap.Texture2D);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.AlphaTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Disable(EnableCap.CullFace);


        matView = Matrix4.LookAt(cameraFrom, cameraTo, Vector3.UnitY);
        Matrix4 mVP = matView * matProjection;

        glShaders[1].Bind();
        glShaders[1].SetUniformMat4("cameraMatrix", mVP, false);
        glSceneTextures[0].Bind();

        SceneDraw myDrawFlags = 0;
        myDrawFlags |= SceneDraw.Geometry;
        // myDrawFlags |= SceneDraw.Collision;
        // myDrawFlags |= SceneDraw.RenderAABB;
        myDrawFlags |= SceneDraw.PointLight;
        myDrawFlags |= SceneDraw.Object;

        scene.Draw(myDrawFlags);

        SwapBuffers();
    }

    private void HandleInputs()
    {
        var mouseState = MouseState;
        lastMousePosition.X = mousePosition.X;
        lastMousePosition.Y = mousePosition.Y;
        
        if (!IsFocused)
        {
            return;
        }

        mousePosition.X = mouseState.X;
        mousePosition.Y = mouseState.Y;

        camLookX -= ((lastMousePosition.X - mousePosition.X) * 0.0174533f) / 4f;
        camLookY -= ((lastMousePosition.Y - mousePosition.Y)) / 4f;
        camLookY = Math.Clamp(camLookY, -90f, 90f);

        float f_x = MathF.Sqrt(MathF.Pow(91, 2) - MathF.Pow(camLookY, 2));

        KeyboardState input = KeyboardState;
        int wKey = input.IsKeyDown(Keys.W) ? 1 : 0;
        int aKey = input.IsKeyDown(Keys.A) ? 1 : 0;
        int sKey = input.IsKeyDown(Keys.S) ? 1 : 0;
        int dKey = input.IsKeyDown(Keys.D) ? 1 : 0;
        int xAxis = (aKey - dKey);
        int yAxis = (wKey - sKey);

        float speed = 64.0f;
        if (xAxis != 0 && yAxis != 0)
        {
            speed = speed * 0.70710678118f;
        }

        //Shift for sprint
        if (input.IsKeyDown(Keys.LeftShift))
        {
            speed = speed * 2;
        }

        if (yAxis != 0)
        {
            cameraFrom.X += ((MathF.Cos(camLookX) * f_x) * 0.0039f) * yAxis * speed;
            cameraFrom.Z += ((MathF.Sin(camLookX) * f_x) * 0.0039f) * yAxis * speed;
            cameraFrom.Y += (-(camLookY * 0.0039f)) * yAxis * speed;
        }

        if (xAxis != 0)
        {
            cameraFrom.X += ((MathF.Cos(camLookX - 1.57079632679f) * f_x) * 0.0039f) * xAxis * speed;
            cameraFrom.Z += ((MathF.Sin(camLookX - 1.57079632679f) * f_x) * 0.0039f) * xAxis * speed;
        }

        //Set Camera To
        cameraTo.X = cameraFrom.X + (MathF.Cos(camLookX) * f_x);
        cameraTo.Z = cameraFrom.Z + (MathF.Sin(camLookX) * f_x);
        cameraTo.Y = cameraFrom.Y - camLookY;
    }
}