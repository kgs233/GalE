using log4net;

namespace GalE.Core;

static public class GEnginMain
{
    public static ILog EnginLogger { get; } = LogManager.GetLogger(typeof(GEnginBase));
    public static void Main()
    {
        GEnginLoop.Init();

        
        Environment.Exit(0);
    }
}
