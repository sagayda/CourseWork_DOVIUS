namespace CourseWork;

public readonly record struct IntBounds(IntPoint Minimum, IntPoint Maximum)
{
    public bool Contains(IntPoint point)
    {
        return point.X >= Minimum.X
            && point.X <= Maximum.X
            && point.Y >= Minimum.Y
            && point.Y <= Maximum.Y;
    }
};
