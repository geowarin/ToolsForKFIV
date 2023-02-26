using System;
using OpenTK.Graphics.OpenGL;

namespace OpenGlKFIV.Rendering.GL;

public class TriangleMesh : IMesh
{
    //Data
    private int _vbo;
    private int _vao;
    private int _vertexCount;

    //Properties
    public int VBO
    {
        get { return _vbo; }
        private set { _vbo = value; }
    }        
    public int VAO
    {
        get { return _vao; }
        private set { _vao = value; }
    }      
    public int VertexCount
    {
        get
        {
            return _vertexCount;
        }
        private set
        {
            _vertexCount = value;
        }
    }

    public TriangleMesh(ref float[] vertices, int vertexCount)
    {
        VertexCount = vertexCount;

        //Construct GL Mesh
        VBO = OpenTK.Graphics.OpenGL.GL.GenBuffer();
        VAO = OpenTK.Graphics.OpenGL.GL.GenVertexArray();

        OpenTK.Graphics.OpenGL.GL.BindVertexArray(VAO);

        OpenTK.Graphics.OpenGL.GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        OpenTK.Graphics.OpenGL.GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

        OpenTK.Graphics.OpenGL.GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 48, 0);   //Position
        OpenTK.Graphics.OpenGL.GL.EnableVertexAttribArray(0);

        OpenTK.Graphics.OpenGL.GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, 48, 12);   //Normal
        OpenTK.Graphics.OpenGL.GL.EnableVertexAttribArray(1);

        OpenTK.Graphics.OpenGL.GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 48, 24);  //Texcoord
        OpenTK.Graphics.OpenGL.GL.EnableVertexAttribArray(2);

        OpenTK.Graphics.OpenGL.GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 48, 32);  //Colour
        OpenTK.Graphics.OpenGL.GL.EnableVertexAttribArray(3);

        OpenTK.Graphics.OpenGL.GL.BindVertexArray(0);
    }

    public void Draw()
    {
        OpenTK.Graphics.OpenGL.GL.BindVertexArray(VAO);
        OpenTK.Graphics.OpenGL.GL.DrawArrays(PrimitiveType.Triangles, 0, _vertexCount);
    }

    //Disposal
    ~TriangleMesh()
    {
        Dispose(false);
    }
    protected void Dispose(bool disposeManagedObjects)
    {
        OpenTK.Graphics.OpenGL.GL.BindVertexArray(0);
        OpenTK.Graphics.OpenGL.GL.DeleteVertexArray(VAO);
        OpenTK.Graphics.OpenGL.GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        OpenTK.Graphics.OpenGL.GL.DeleteBuffer(VBO);

        if (disposeManagedObjects)
        {

        }
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}