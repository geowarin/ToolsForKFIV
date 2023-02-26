using System;

namespace OpenGlKFIV.Rendering.GL;

public interface IMesh : IDisposable
{
    int VBO { get; }
    int VAO { get; }

    void Draw();
}