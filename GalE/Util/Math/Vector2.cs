namespace GalE.Util.Math;

public class Vector2 
{
    public readonly static Vector2 up = new(0, 1);
    public readonly static Vector2 down = new(0, -1);
    public readonly static Vector2 left = new(-1, 0);
    public readonly static Vector2 right = new(1, 0);
    public readonly static Vector2 one = new(1, 1);
    public readonly static Vector2 zero = new(0, 0);

#pragma warning disable IDE1006
    public int x { get; set; }
    public int y { get; set; }
#pragma warning restore IDE1006

    public Vector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
