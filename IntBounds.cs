namespace CourseWork;

public readonly record struct IntBounds(IntPoint Minimum, IntPoint Maximum)
{
    public int Width => Maximum.X - Minimum.X;
    public int Height => Maximum.Y - Minimum.Y;

    public bool Contains(IntPoint point)
    {
        return point.X >= Minimum.X && point.X <= Maximum.X && point.Y >= Minimum.Y && point.Y <= Maximum.Y;
    }

    public override string ToString()
    {
        return $"({Width}x{Height})";
    }
};
