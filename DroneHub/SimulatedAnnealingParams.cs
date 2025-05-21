namespace CourseWork;

public class SimulatedAnnealingParams
{
    public int Seed { get; set; } = 0;
    public int Iterations { get; init; } = 1000;
    public int MaxStagnationIterations { get; init; } = -1;

    public double InitialTemperature { get; init; } = 100.0d;
    public double CoolingRate { get; init; } = 0.995d;
}
