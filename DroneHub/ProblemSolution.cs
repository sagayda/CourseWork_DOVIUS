namespace CourseWork.DroneHub;

public class ProblemSolution
{
    public IReadOnlyCollection<IntPoint>? History { get; init; } = null;

    public int IterationsTook { get; }
    public TimeSpan TimeTook { get; }

    public IntPoint Result { get; }
    public double ResultObjective { get; }

    public ProblemSolution(
        IntPoint result,
        double resultObjective,
        TimeSpan timeTook,
        int iterationsTook
    )
    {
        Result = result;
        ResultObjective = resultObjective;
        TimeTook = timeTook;
        IterationsTook = iterationsTook;
    }
}
