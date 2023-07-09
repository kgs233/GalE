namespace GalE.Core;


public delegate void EnginUpdateHandler(GEnginBase Engin);

public abstract class GEnginBase : GEnginObject, IDisposable
{
    protected CheckInit Inited = new();

    public abstract string GameName { get; set; }
    public abstract string GameOwner { get; set; }
    public abstract GEnginWindow GameBaseWindow { get; set; }
    public abstract Util.Version GameVersion { get; set; }

    public string RunOS { get; } = Environment.OSVersion.Platform.ToString();
    public string RunOSLanguage { get; } = System.Globalization.CultureInfo.CurrentCulture.Name;

    public abstract void Init();
    public abstract void Run();
    public abstract void Update();
}