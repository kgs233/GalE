using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace GalE;

/// <summary>
/// The Priont Utils
/// </summary>

public static unsafe class PtrUtils
{
    public static byte* StrToBytePointer(string str)
    {
        return ToPointer<byte>(Encoding.UTF8.GetBytes(str));
    }

    public static T* ToPointer<T>(byte[] Safe)
    {
        T* pointerByte = (T*)Marshal.AllocHGlobal(Safe.Length);
        Marshal.Copy(Safe, 0, (IntPtr)pointerByte, Safe.Length);
        return pointerByte;
    }
}