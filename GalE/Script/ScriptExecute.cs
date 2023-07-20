using GalE.Script;
using System.Reflection;

namespace GalE;

internal static class ScriptExecute
{
    private readonly static Type Base = typeof(ScriptBase);
    private readonly static Type[] Types = Assembly.GetCallingAssembly().GetTypes();

    private static List<ScriptBase> ScriptList = new();

    internal static void Init()
    {
        foreach(Type type in Types)
        {
            Type? baseType = type.BaseType;
            if (baseType != null)
            {
                if (baseType.Name == Base.Name)
                {
                    Type? objtype = Type.GetType(type.FullName, true);
                    object? obj = Activator.CreateInstance(objtype);
                    if (obj != null)
                    {
                        ScriptBase? info = obj as ScriptBase;
                        ScriptList.Add(info);
                    }
                }
            }
        }
    }

    internal static void OnInit(GEngin engin)
    {
        foreach (var script in ScriptList)
        {
            script.Init(engin);
        }
    }

    internal static void OnUpdate(GEngin engin)
    {
        foreach (var script in ScriptList)
        {
            script.Update(engin);
        }
    }
}
