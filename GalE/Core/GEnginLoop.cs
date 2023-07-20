namespace GalE.Core;

public static class GEnginLoop
{
    private static GEnginBase Engin;
    public static bool Running { get; private set; } = false;

    public static void Init()
    {
        ScriptExecute.Init();
        Engin = new GEngin();
        Running = true;
        Engin.Init();
    }

    public static void Run()
    {
        while(Running)
        {
            Engin.Update();
            if(!Engin.GameBaseWindow.Runing)
            {
                Exit();
                break;
            }
        }
    }

    public static void Exit() => Exit(0);
    public static void Exit(int exitArg)
    {
        Engin.Dispose();
        Environment.Exit(exitArg);
    }
}

public static class GEnginMain
{
    public static void Main(string[] args)
    {
        GEnginLoop.Init();
        GEnginLoop.Run();
    }
}