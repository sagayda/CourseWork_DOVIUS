namespace CourseWork.DroneHub.Experiments;

public class StagnationImpactExperiment : ExperimentBase
{
    public IList<float> Alphas { get; set; }

    public override int LaunchesPerIteration => 10;
    public override int Iterations => Alphas.Count;

    public override bool RunLS => false;
    public override bool RunSA => true;

    public StagnationImpactExperiment(IList<float> alphas)
    {
        this.Alphas = alphas;
    }

    protected override IEnumerable<Run> GetDataForIteration(int iteration, out IReadOnlyDictionary<string, object>? annotations)
    {
        Random random = new(Seed);
        ProblemGenerator generator = new()
        {
            Density = 0.2f,
            Deviation = 0,
            Volume = 2f,
        };

        List<Run> runs = [];
        int maxStagnationIterationsToReport = -1;
        for (int i = 0; i < LaunchesPerIteration; i++)
        {
            generator.Seed = Seed + i;

            var problem = generator.Generate();
            var n = Math.Sqrt(problem.Bounds.Width * problem.Bounds.Height);
            var maxStagnationIterations = Convert.ToInt32(Alphas[iteration] * n * Math.Log2(n));

            if (maxStagnationIterationsToReport == -1)
                maxStagnationIterationsToReport = maxStagnationIterations;

            runs.Add(
                new()
                {
                    SimulatedAnnealingParams = new() { Seed = Seed + 1, MaxStagnationIterations = maxStagnationIterations },
                    Problem = problem,
                }
            );
        }

        annotations = new Dictionary<string, object>()
        {
            { "Alpha", Alphas[iteration] },
            { "I_Stagnation", maxStagnationIterationsToReport },
        };

        return runs;
    }
}
