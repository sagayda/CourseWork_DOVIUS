﻿using System.Diagnostics;

namespace CourseWork.DroneHub;

public class SimulatedAnnealing : ISolutionAlgorithm
{
    private Random? _workinRandom;

    public SimulatedAnnealingParams Parameters { get; set; }

    public SimulatedAnnealing(SimulatedAnnealingParams parameters)
    {
        Parameters = parameters;
    }

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

        _workinRandom = new(Parameters.Seed);
        List<IntPoint>? history = saveHistory ? [] : null;

        var bounds = problem.Bounds;

        IntPoint current = new(
            _workinRandom.Next(bounds.Minimum.X, bounds.Maximum.X + 1),
            _workinRandom.Next(bounds.Minimum.Y, bounds.Maximum.Y + 1)
        );
        double currentObjective = problem.CalculateObjectiveFor(current);
        history?.Add(current);

        int stagnationIterations = 0;

        IntPoint best = current;
        double bestObjective = currentObjective;

        double temperature = Parameters.InitialTemperature;

        int i = 0;
        for (; i < Parameters.Iterations; i++)
        {
            stagnationIterations++;

            var neighbour = GetNeighbour(current, bounds);
            var neighbourObjective = problem.CalculateObjectiveFor(neighbour);

            var deltaObjective = neighbourObjective - currentObjective;

            bool accept = false;
            if (deltaObjective > 0)
            {
                accept = true;
            }
            else
            {
                var acceptance = Math.Exp(deltaObjective / temperature);
                var r = _workinRandom.NextDouble();

                if (r < acceptance)
                    accept = true;
            }

            if (accept)
            {
                current = neighbour;
                currentObjective = neighbourObjective;
                history?.Add(current);

                if (currentObjective > bestObjective)
                {
                    stagnationIterations = 0;
                    best = current;
                    bestObjective = currentObjective;
                }
            }

            temperature *= Parameters.CoolingRate;

            if (Parameters.MaxStagnationIterations >= 0 && stagnationIterations >= Parameters.MaxStagnationIterations)
                break;
        }

        stopwatch.Stop();

        return new ProblemSolution(best, bestObjective, stopwatch.Elapsed, i) { History = history };
    }

    private IntPoint GetNeighbour(IntPoint forPoint, IntBounds bounds)
    {
        IntPoint neighbour = forPoint;

        do
        {
            neighbour = new IntPoint(forPoint.X + _workinRandom!.Next(-1, 2), forPoint.Y + _workinRandom!.Next(-1, 2));
        } while (neighbour == forPoint || bounds.Contains(neighbour) == false);

        return neighbour;
    }
}
