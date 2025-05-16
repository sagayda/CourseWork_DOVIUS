namespace CourseWork;

public readonly record struct IntPoint(int X, int Y)
{
    public double DistanceTo(IntPoint other)
    {
        return Math.Sqrt(Math.Pow(other.X - X, 2) + Math.Pow(other.Y - Y, 2));
    }

    public static IntPoint operator +(IntPoint a, IntPoint b) => new IntPoint(a.X + b.X, a.Y + b.Y);

    public static IntPoint operator -(IntPoint a, IntPoint b) => new IntPoint(a.X - b.X, a.Y - b.Y);
}
