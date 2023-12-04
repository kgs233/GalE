using GalE;
using GalE.Core;
using GalE.Script;

namespace Test;

public class Test : ScriptBase
{
    public override void Init(GEngin engin)
    {
        engin.GameBaseWindow = new(size: new(800, 900));
    }
}
