namespace CourseWork.DroneHub.Experiments;

public abstract class ExperimentBase
{
    public int Seed { get; set; } = new Random().Next();

    public abstract int LaunchesPerIteration { get; }
    public abstract int Iterations { get; }
    public abstract bool RunLS { get; }
    public abstract bool RunSA { get; }

    public (List<ExperimentResult>? SAResults, List<ExperimentResult>? LSResults) Execute()
    {
        List<ExperimentResult>? SAResults = RunSA ? [] : null;
        List<ExperimentResult>? LSResults = RunLS ? [] : null;

        SimulatedAnnealing sa = new(new());
        LocalSearch ls = new();

        for (int i = 0; i < Iterations; i++)
        {
            var runs = GetDataForIteration(i, out IReadOnlyDictionary<string, object>? annotations);

            if (RunSA)
                SAResults!.Add(ExecuteSA(runs, sa, annotations));

            if (RunLS)
                LSResults!.Add(ExecuteLS(runs, ls, annotations));
        }

        return (SAResults, LSResults);
    }

    protected abstract IEnumerable<Run> GetDataForIteration(int iteration, out IReadOnlyDictionary<string, object>? annotations);

    private ExperimentResult ExecuteLS(IEnumerable<Run> runs, LocalSearch ls, IReadOnlyDictionary<string, object>? annotations)
    {
        List<ProblemSolution> results = [];

        foreach (var run in runs)
            results.Add(ls.Solve(run.Problem));

        TimeSpan totalTime = new(0);
        double totalObjective = 0;
        foreach (var result in results)
        {
            totalTime += result.TimeTook;
            totalObjective += result.ResultObjective;
        }

        return new(totalTime / results.Count, totalObjective / results.Count, annotations);
    }

    private ExperimentResult ExecuteSA(IEnumerable<Run> runs, SimulatedAnnealing sa, IReadOnlyDictionary<string, object>? annotations)
    {
        List<ProblemSolution> results = [];

        foreach (var run in runs)
        {
            sa.Parameters = run.SimulatedAnnealingParams!;
            results.Add(sa.Solve(run.Problem));
        }

        TimeSpan totalTime = new(0);
        double totalObjective = 0;
        foreach (var result in results)
        {
            totalTime += result.TimeTook;
            totalObjective += result.ResultObjective;
        }

        return new(totalTime / results.Count, totalObjective / results.Count, annotations);
    }

    protected class Run
    {
        public required ProblemParams Problem { get; init; }
        public SimulatedAnnealingParams? SimulatedAnnealingParams { get; init; }
    }
}

public record class ExperimentResult
{
    public TimeSpan AvarageTime { get; }
    public double AvarageObjective { get; }

    public IReadOnlyDictionary<string, object>? Annotations { get; }

    public ExperimentResult(TimeSpan avarageTime, double avarageObjective, IReadOnlyDictionary<string, object>? annotations)
    {
        AvarageTime = avarageTime;
        AvarageObjective = avarageObjective;
        Annotations = annotations;
    }
}
