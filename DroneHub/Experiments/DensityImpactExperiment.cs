namespace CourseWork.DroneHub.Experiments;

public class DensityImpactExperiment : ExperimentBase
{
    public IList<float> GeneratorDensity { get; set; }

    public override int LaunchesPerIteration => 10;
    public override int Iterations => GeneratorDensity.Count;

    public override bool RunLS => true;
    public override bool RunSA => true;

    public DensityImpactExperiment(IList<float> generatorDensity)
    {
        GeneratorDensity = generatorDensity;
    }

    protected override IEnumerable<Run> GetDataForIteration(int iteration, out IReadOnlyDictionary<string, object>? annotations)
    {
        Random random = new(Seed);
        SimulatedAnnealingParams saParams = new() { Seed = Seed };
        ProblemGenerator generator = new()
        {
            Density = GeneratorDensity[iteration],
            Deviation = 0,
            Volume = 1f,
        };

        List<Run> runs = [];
        for (int i = 0; i < LaunchesPerIteration; i++)
        {
            generator.Seed = Seed + i;
            saParams.Seed = Seed + i;
            runs.Add(new() { SimulatedAnnealingParams = saParams, Problem = generator.Generate() });
        }

        annotations = new Dictionary<string, object>()
        {
            { "Density", GeneratorDensity[iteration] },
            { "Points", runs.FirstOrDefault()?.Problem.Points.Length ?? default },
        };

        return runs;
    }
}
