namespace CourseWork.DroneHub;

public interface ISolutionAlgorithm
{
    public ProblemSolution Solve(ProblemParams problem, bool saveHistory = false);
}
