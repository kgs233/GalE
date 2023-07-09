using GalE.Core;

namespace GalE;

public class GEnginObject : IDisposable
{
    public static List<GEnginObject> EnginObjectList { get; internal set; } = new();
    public string ObjectName { get; init; }
    public int ObjectID { get; init; }

    public GEnginObject() : this("", EnginObjectList.Count - 1) {}
    public GEnginObject(string objectName) : this(objectName, EnginObjectList.Count - 1) {}
    public GEnginObject(int objectID) : this("", objectID) {}
    public GEnginObject(string objectName, int objectID)
    {
        ObjectName = objectName;
        ObjectID = objectID;
        EnginObjectList.Add(this);
    }

    public static GEnginObject? FindByObjectById(int id)
    {
        GEnginObject? enginObject = EnginObjectList.Where(enginObject => enginObject.ObjectID == id).FirstOrDefault();
        if(enginObject is null)
        {
            GEnginMain.EnginLogger.Error("NotFound" + id);
            return null;
        }
        else
        {
            return enginObject;
        }
    }
    public static dynamic FindObjectByName(string objectName)
    {
        List<GEnginObject> objectList = EnginObjectList.Where(enginObject => enginObject.ObjectName == objectName).ToList();
        return objectList.Count == 1 ? objectList[0] : objectList;
    }

    public override string ToString()
    {
        return ObjectName;
    }
    public void Dispose()
    {
        EnginObjectList.Remove(this);
        GC.SuppressFinalize(this);
    }
}