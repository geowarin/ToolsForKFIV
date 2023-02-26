using System;
using System.Linq;
using FormatKFIV.Asset;
using FormatKFIV.FileFormat;
using OpenGlKFIV.Utility;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ResourceKFIV.Asset;
using ResourceKFIV.Filesystem;

namespace OpenGlKFIV.UI.Control;

public class ToolFFModel : GameWindow
{
    public ToolFFModel(int width, int height, string title, ModelAsset modelAsset) :
        base(GameWindowSettings.Default, new NativeWindowSettings { Size = (width, height), Title = title })
    {
        SetModelFile(modelAsset.Model, modelAsset.Texture);
    }

    private Matrix4 matrixProjection;
    private Matrix4 matrixView;
    private Vector3 vectorCEye = new(0, 0, 16);
    private float cameraX = 225 * 0.0174533f, cameraY = 45;
    private float cameraZoom = 0.2f;
    private float mouseLastX, mouseLastY;

    private Model model;
    private Texture texture;

    private GLShader shader3DNormTexColour;
    private GLShader shader3DColour;
    private GLTexture textureFile;
    private GLModel modelGrid;
    private GLModel modelFile;

    private void Export()
    {
        var fiFormat = ResourceManager.GetExportableModelFormats().First();
        var path = "export/model";
        if (model != null)
        {
            FIFormat<Model> mdlFmt = fiFormat;
            mdlFmt.SaveToFile(path, model);
        }

        if (texture != null)
        {
            FIFormat<Texture> texFmt = new FFTexturePNG();
            texFmt.SaveToFile(path + ".png", texture);
        }
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        //Initialize Shaders
        shader3DColour = new GLShader(
            "Resource/shd_3DColour.vs",
            "Resource/shd_3DColour.fs"
        );
        shader3DNormTexColour = new GLShader(
            "Resource/shd_3DNormTexColour.vs",
            "Resource/shd_3DNormTexColour.fs"
        );

        //Initialize Models
        //modelAxis = GLModel.Generate3DAxis();
        modelGrid = GLModel.Generate3DGrid();

        //Initialize Camera
        double f_x = Math.Sqrt(Math.Pow(91, 2) - Math.Pow(cameraY, 2)) * 0.2;
        vectorCEye.X = (float)(Math.Cos(cameraX) * f_x);
        vectorCEye.Z = (float)(Math.Sin(cameraX) * f_x);
        vectorCEye.Y = cameraY * 0.2f;
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
        matrixProjection = Matrix4.CreatePerspectiveFieldOfView(0.7f, e.Width / (float)e.Height, 0.1f, 8192f);
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
        //Compute View Matrix/MVP Matrix
        matrixView = Matrix4.LookAt(vectorCEye, Vector3.Zero, Vector3.UnitY);

        //Do OpenGL
        GL.ClearColor(Settings.Instance.mtBgCC.ToColor());
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Multisample);
        GL.Enable(EnableCap.AlphaTest);
        GL.Disable(EnableCap.CullFace);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        //Draw Editor Shit
        shader3DColour.Bind();
        shader3DColour.SetUniformMat4("cameraMatrix", matrixView * matrixProjection, false);

        if (Settings.Instance.mtShowGridAxis)
        {
            modelGrid.DrawLines();
        }

        //Draw Loaded File
        if (modelFile != null)
        {
            shader3DNormTexColour.Bind();
            shader3DNormTexColour.SetUniformMat4("cameraMatrix", matrixView * matrixProjection, false);
            shader3DNormTexColour.SetUniformMat4("worldMatrix", Matrix4.Identity, false);
            shader3DNormTexColour.SetUniformInt1("sDiffuse", 0);

            if (textureFile != null)
            {
                textureFile.Bind();
            }

            if (modelFile != null)
            {
                for (int i = 0; i < modelFile.MeshCount; ++i)
                {
                    modelFile.DrawTriangleMesh(i);
                }
            }
        }

        SwapBuffers();
    }

    private void HandleInputs()
    {
        var mouseState = MouseState;
        bool IsInside = IsFocused;
        var e = mouseState.Position;

        //Do Camera Rotation
        if (mouseState[MouseButton.Left] && IsInside)
        {
            cameraX -= (mouseLastX - e.X) / 32f;
            cameraY -= (mouseLastY - e.Y);
            cameraY = Math.Clamp(cameraY, -90, 90);
        }

        //Do Camera Zoom
        if (mouseState[MouseButton.Right] && IsInside)
        {
            cameraZoom -= (mouseLastY - e.Y) / 100f;
            cameraZoom = Math.Clamp(cameraZoom, 0.02f, 32f);
        }

        double f_x = Math.Sqrt(Math.Pow(91, 2) - Math.Pow(cameraY, 2)) * cameraZoom;

        vectorCEye.X = (float)(Math.Cos(cameraX) * f_x);
        vectorCEye.Z = (float)(Math.Sin(cameraX) * f_x);
        vectorCEye.Y = cameraY * cameraZoom;

        mouseLastX = e.X;
        mouseLastY = e.Y;
    }

    private void SetModelFile(Model mod, Texture tex = null)
    {
        if (modelFile != null)
        {
            modelFile.Destroy();
            modelFile = null;
            model = null;
        }

        if (textureFile != null)
        {
            textureFile.Destroy();
            textureFile = null;
        }

        if (tex != null)
        {
            textureFile = GLTexture.GenerateFromAsset(tex);
            texture = tex;
        }
        else
        {
            textureFile = GLTexture.Generate44White();
        }

        modelFile = GLModel.GenerateFromAsset(mod);
        model = mod;

        int tsCounter = 0;
        foreach (Model.TextureSlot ts in model.TextureSlots)
        {
            Console.WriteLine($"Texture Slot [{tsCounter}]: {ts.slotKey.ToString("X8")}");

            tsCounter++;
        }
    }
}