namespace CourseWork.DroneHub.Experiments;

public class VolumeImpactExperiment : ExperimentBase
{
    public IList<float> GeneratorVolumes { get; set; }

    public override int LaunchesPerIteration => 10;
    public override int Iterations => GeneratorVolumes.Count;

    public override bool RunLS => true;
    public override bool RunSA => true;

    public VolumeImpactExperiment(IList<float> generatorVolumes)
    {
        GeneratorVolumes = generatorVolumes;
    }

    protected override IEnumerable<Run> GetDataForIteration(int iteration, out IReadOnlyDictionary<string, object>? annotations)
    {
        Random random = new(Seed);
        SimulatedAnnealingParams saParams = new() { Seed = Seed };
        ProblemGenerator generator = new()
        {
            Density = 0.2f,
            Deviation = 0,
            Volume = GeneratorVolumes[iteration],
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
            { "Volume", GeneratorVolumes[iteration] },
            { "Size", runs.FirstOrDefault()?.Problem.Bounds ?? default },
        };

        return runs;
    }
}
