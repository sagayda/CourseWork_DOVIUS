namespace CourseWork.DroneHub.Experiments;

public class TemperatureImpactExperiment : ExperimentBase
{
    public IList<float> Temperatures { get; set; }

    public override int LaunchesPerIteration => 10;
    public override int Iterations => Temperatures.Count;

    public override bool RunLS => false;
    public override bool RunSA => true;

    public TemperatureImpactExperiment(IList<float> temperatures)
    {
        Temperatures = temperatures;
    }

    protected override IEnumerable<Run> GetDataForIteration(int iteration, out IReadOnlyDictionary<string, object>? annotations)
    {
        Random random = new(Seed);
        SimulatedAnnealingParams saParams = new() { Seed = Seed, InitialTemperature = Temperatures[iteration] };
        ProblemGenerator generator = new()
        {
            Density = 0.2f,
            Deviation = 0,
            Volume = 2f,
        };

        List<Run> runs = [];
        for (int i = 0; i < LaunchesPerIteration; i++)
        {
            generator.Seed = Seed + i;
            saParams.Seed = Seed + i;
            runs.Add(new() { SimulatedAnnealingParams = saParams, Problem = generator.Generate() });
        }

        annotations = new Dictionary<string, object>() { { "Temperature", Temperatures[iteration] } };

        return runs;
    }
}
