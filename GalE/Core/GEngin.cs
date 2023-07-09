using GalE.Core;

namespace GalE;

public record GEnginInitArg(string Name, string Owner, Util.Version Version);

public class GEngin : GEnginBase
{
    public override string GameName { get; set; }
    public override string GameOwner { get; set; }
    public override GEnginWindow GameBaseWindow { get; set; }
    public override Util.Version GameVersion { get; set; }

    public GEngin() : this(new("", "", new(new(0, 0, 0))))
    { }

    public GEngin(GEnginInitArg initArg)
    {
        ObjectID = 0;
        ObjectName = "Game Engin";
        GameName = initArg.Name;
        GameOwner = initArg.Owner;
        GameBaseWindow = new();
        GameVersion = new(new(0, 0, 0));
    }

    public override void Init()
    {
        GameBaseWindow.RandererEvent += Update;
        GameBaseWindow.Init(this);
        Inited.Init();
    }

    public override void Run()
    {
        Inited.Check();
        GameBaseWindow.Run();
    }

    public override void Update()
    {
    }

    public new void Dispose()
    {
        Inited.Check();
        GameBaseWindow.Dispose();
        base.Dispose();
    }
}