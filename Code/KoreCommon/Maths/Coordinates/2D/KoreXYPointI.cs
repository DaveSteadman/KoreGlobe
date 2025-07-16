
// KoreInt2D: A class to store a positive 2D integer size.

namespace KoreCommon;

public struct KoreXYPointI
{
    public int X
    {
        get => X;
        set => X = value < 0 ? 0 : value;
    }

    public int Y
    {
        get => Y;
        set => Y = value < 0 ? 0 : value;
    }

    public KoreXYPointI(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

