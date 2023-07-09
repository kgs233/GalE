namespace GalE.Core;

public static class GEnginLoop
{
#pragma warning disable CS8618

    private static GEnginBase GEngin;
#pragma warning restore CS8618
    public static bool IsRunning { get; private set; } = false;

    public static void Init()
    {
        GEngin = new GEngin();
        IsRunning = true;
        GEngin.Init();
    }

    internal static void Update()
    {
    }

    public static void Exit()
    {
    }

    public static void GetGEngin(GEngin Engin)
    {
    }
}