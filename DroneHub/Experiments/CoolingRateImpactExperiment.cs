namespace CourseWork.DroneHub.Experiments;

public class CoolingRateImpactExperiment : ExperimentBase
{
    public IList<float> CoolingRates { get; set; }

    public override int LaunchesPerIteration => 10;
    public override int Iterations => CoolingRates.Count;

    public override bool RunLS => false;
    public override bool RunSA => true;

    public CoolingRateImpactExperiment(IList<float> coolingRates)
    {
        CoolingRates = coolingRates;
    }

    protected override IEnumerable<Run> GetDataForIteration(int iteration, out IReadOnlyDictionary<string, object>? annotations)
    {
        Random random = new(Seed);
        SimulatedAnnealingParams saParams = new() { Seed = Seed, CoolingRate = CoolingRates[iteration] };
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

        annotations = new Dictionary<string, object>() { { "Cooling rate", CoolingRates[iteration] } };

        return runs;
    }
}
