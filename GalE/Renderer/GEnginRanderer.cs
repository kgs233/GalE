using GalE.Core;

namespace GalE.Renderer;

public abstract class GEnginRenderer : IDisposable
{
    public CheckInit Inited = new();
    public abstract void Init(GEnginWindow window);
    public abstract void Dispose();
}