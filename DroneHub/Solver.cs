using System.Diagnostics.CodeAnalysis;

namespace CourseWork.DroneHub;

public class Solver
{
    public required ISolutionAlgorithm Algorithm { get; set; }
    public required ProblemParams Problem { get; set; }

    public ProblemSolution? LastSolution { get; private set; }

    public bool Solve([NotNullWhen(false)] out string? errorMessage)
    {
        errorMessage = null;

        if (Algorithm is null || Problem is null)
        {
            errorMessage = "No algorithm prowided";
            return false;
        }

        if (Problem is null)
        {
            errorMessage = "No problem provided";
            return false;
        }

        ProblemSolution? result = null;

        try
        {
            result = Algorithm.Solve(Problem);
        }
        catch (System.Exception e)
        {
            errorMessage = "An error occurred while solving the problem:\n" + e.Message;
            return false;
        }

        if (result is null)
        {
            Console.WriteLine("The algorithm did not produce any results");
            return false;
        }

        LastSolution = result;
        return true;
    }
}
