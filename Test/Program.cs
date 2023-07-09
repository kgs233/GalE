using GalE;
using GalE.Core;

internal class Program
{
    private static void Main(string[] args)
    {
        using GEngin engin = new(new("", "", new(new(0, 0, 0))));
        engin.Init();
        engin.Run();
    }
}