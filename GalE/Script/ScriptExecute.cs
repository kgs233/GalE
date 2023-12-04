using GalE.Core;
using GalE.Script;
using System.Reflection;

namespace GalE.Script;

public enum EnginEvent
{
    Init,
    AfterInit,
    Update,
    AfterUpdate,
    Exit,
}

public static class ScriptExecute
{
    private readonly static Type Base = typeof(ScriptBase);
    private readonly static Type[] Types = Assembly.GetCallingAssembly().GetTypes();

    private static List<ScriptBase> ScriptList = new();

    public static void Init()
    {
        foreach(Type type in Types)
        {
            Type? baseType = type.BaseType;
            while(baseType != null)
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
                    break;
                }
                else
                {
                    baseType = baseType.BaseType;
                }
            }
        }
    }

    public static void ExeEvent(GEngin engin, EnginEvent @event)
    {
        foreach (var script in ScriptList)
        {
            switch(@event)
            {
                case EnginEvent.Init:
                    script.Init(engin);
                break;

                case EnginEvent.AfterInit:
                    script.AfterInit(engin);
                break;

                case EnginEvent.Update:
                    script.Update(engin);
                break;

                case EnginEvent.AfterUpdate:
                    script.AfterUpdate(engin);
                break;

                case EnginEvent.Exit:
                    script.Exit(engin);
                break;
            }
        }
    }
}
