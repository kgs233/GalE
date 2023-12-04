using GalE.Script;

namespace GalE.Core;

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
        ScriptExecute.ExeEvent(this, EnginEvent.Init);
        GameBaseWindow.Init(this);
        Inited.Init();
    }

    public override void Update()
    {
        Inited.Check();
        ScriptExecute.ExeEvent(this, EnginEvent.Update);
        GameBaseWindow.Update();
        ScriptExecute.ExeEvent(this, EnginEvent.AfterUpdate);
    }

    public new void Dispose()
    {
        Inited.Check();
        GameBaseWindow.Dispose();
        base.Dispose();
        Runing = false;
    }
}