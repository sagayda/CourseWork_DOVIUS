using System.Diagnostics;

namespace CourseWork.DroneHub;

public class LocalSearch : ISolutionAlgorithm
{
    private static IntPoint[] _directions = [new IntPoint(1, 0), new IntPoint(-1, 0), new IntPoint(0, 1), new IntPoint(0, -1)];

    public ProblemSolution Solve(ProblemParams problem, bool saveHistory = false)
    {
        if (problem.Validate(out string? error) == false)
            throw new InvalidDataException(error);

        return Execute(problem, saveHistory);
    }

    private ProblemSolution Execute(ProblemParams problem, bool saveHistory)
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        List<IntPoint>? history = saveHistory ? [] : null;
        var bounds = problem.Bounds;

        int i = 0;

        IntPoint current = default;
        double currentObjective = default;

        IntPoint bestNeighbour = CalculateCenterOfMass(problem.Points);
        double bestNeighbourObjective = problem.CalculateObjectiveFor(current);

        do
        {
            i++;
            current = bestNeighbour;
            currentObjective = bestNeighbourObjective;
            history?.Add(current);

            bestNeighbourObjective = -1;

            foreach (var direction in _directions)
            {
                var neighbour = current + direction;
                if (bounds.Contains(neighbour) == false)
                    continue;

                var neighbourObjective = problem.CalculateObjectiveFor(neighbour);
                if (neighbourObjective > bestNeighbourObjective)
                {
                    bestNeighbour = neighbour;
                    bestNeighbourObjective = neighbourObjective;
                }
            }
        } while (bestNeighbourObjective > currentObjective);

        stopwatch.Stop();
        return new ProblemSolution(current, currentObjective, stopwatch.Elapsed, i) { History = history };
    }

    private IntPoint CalculateCenterOfMass(IEnumerable<DeliveryPoint> points)
    {
        float totalDeliveries = points.Sum(point => point.Deliveries);

        float xCenter = 0;
        float yCenter = 0;

        foreach (var point in points)
        {
            xCenter += point.Coordinates.X * Convert.ToInt32(point.Deliveries);
            yCenter += point.Coordinates.Y * Convert.ToInt32(point.Deliveries);
        }

        xCenter /= totalDeliveries;
        yCenter /= totalDeliveries;

        return new IntPoint(Convert.ToInt32(xCenter), Convert.ToInt32(yCenter));
    }
}
