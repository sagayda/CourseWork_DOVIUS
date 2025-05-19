using System.CommandLine;
using CourseWork.DroneHub;
using Newtonsoft.Json;

namespace CourseWork;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        // FileInfo ff = new("/mnt/D/sagayda/Si_Pee_Eye/SEM6/cw/test.txt");
        // using var fs = ff.CreateText();
        // using JsonTextWriter wr = new(fs) { Formatting = Formatting.Indented };
        // ProblemGenerator generator = new()
        // {
        //     Frequency = 10f,
        //     Volume = 0.2f,
        //     Density = 0.1f,
        //     Deviation = 0.4f,
        // };
        //
        // var gen = generator.Generate();
        // JsonSerializer ser = new();
        // ser.Serialize(wr, gen);

        // var res = JsonConvert.SerializeObject(gen, Formatting.Indented);
        // Console.WriteLine(res);

        var rootCommand = new RootCommand(
            "Applications for solving the drone hub problem - coursework"
        )
        {
            CreateSolveCommand(),
            CreateGenerateCommand(),
        };

        return await rootCommand.InvokeAsync(args);
    }

    private static Command CreateSolveCommand()
    {
        var seedOption = new Option<int>(
            name: "--seed",
            description: "Seed for randomizer",
            getDefaultValue: () => DateTime.Now.Second
        );
        seedOption.AddAlias("-s");

        var iterationsOption = new Option<uint>("--iterations", "Count of iterations");
        iterationsOption.SetDefaultValue(1000u);
        iterationsOption.AddAlias("-i");

        var temperatureOption = new Option<double>("--temperature", "Initial temperature");
        temperatureOption.SetDefaultValue(100d);
        temperatureOption.AddAlias("-t");

        var simulatedAnnealingCommand = new Command(
            "simulated-annealing",
            "Use simulated-annealing algorithm"
        )
        {
            iterationsOption,
            temperatureOption,
            seedOption,
        };
        simulatedAnnealingCommand.AddAlias("sa");
        var localSearchCommand = new Command("local-search", "Use local-search algorithm");
        localSearchCommand.AddAlias("ls");

        var minXOption = new Option<int>("--min-x", "Minimum X coordinate of the bounds");
        minXOption.SetDefaultValue(0);
        var minYOption = new Option<int>("--min-y", "Minimum Y coordinate of the bounds");
        minYOption.SetDefaultValue(0);
        var maxXOption = new Option<int>("--max-x", "Maximum X coordinate of the bounds");
        maxXOption.SetDefaultValue(64);
        var maxYOption = new Option<int>("--max-y", "Maximum Y coordinate of the bounds");
        maxYOption.SetDefaultValue(64);

        var droneDistanceOption = new Option<float>("--distance", "Drone flight distance");
        droneDistanceOption.SetDefaultValue(16f);
        droneDistanceOption.AddAlias("-d");
        // droneDistanceOption.AddValidator(
        //     (result) =>
        //     {
        //         if (result.GetValueForOption(droneDistanceOption) <= 0)
        //             result.ErrorMessage = "Drone distance must be greater than zero";
        //     }
        // );

        #region manual input
        var xArrayOption = new Option<int[]>("-x", "List of X coordinates for delivery points")
        {
            IsRequired = true,
            AllowMultipleArgumentsPerToken = true,
        };
        var yArrayOption = new Option<int[]>("-y", "List of Y coordinates for delivery points")
        {
            IsRequired = true,
            AllowMultipleArgumentsPerToken = true,
        };
        var wArrayOption = new Option<uint[]>(
            "-w",
            "List of delivery quantities for delivery points"
        )
        {
            IsRequired = true,
            AllowMultipleArgumentsPerToken = true,
        };

        var solveManualCommand = new Command("manual", "Enter data manualy")
        {
            xArrayOption,
            yArrayOption,
            wArrayOption,
            minXOption,
            minYOption,
            maxXOption,
            maxYOption,
            droneDistanceOption,
            simulatedAnnealingCommand,
            localSearchCommand,
        };

        #endregion

        #region file input
        var filePathArgument = new Argument<FileInfo>("path", "Path to file");
        // filePathArgument.AddValidator(
        //     (result) =>
        //     {
        //         // var file = result.GetValueForOption(filePathOption);
        //         var file = result.GetValueForArgument(filePathArgument);
        //
        //         if (file is null || file.Exists == false)
        //         {
        //             result.ErrorMessage = "TODO ERR";
        //         }
        //     }
        // );

        var solveFileCommand = new Command("file", "Read data from json file")
        {
            filePathArgument,
            simulatedAnnealingCommand,
            localSearchCommand,
        };
        #endregion

        DroneHubProblemPramsBinder paramsBinder = new(
            xArrayOption,
            yArrayOption,
            wArrayOption,
            minXOption,
            minYOption,
            maxXOption,
            maxYOption,
            droneDistanceOption,
            filePathArgument
        );

        simulatedAnnealingCommand.SetHandler(
            (ProblemParams problemParams, uint iterations, double temperature, int seed) =>
            {
                SimulatedAnnealingParams parameters = new()
                {
                    Iterations = Convert.ToInt32(iterations),
                    InitialTemperature = temperature,
                    Seed = seed,
                };

                Console.WriteLine("Running simulated-annealing solver...");
                RunSASolver(problemParams, parameters);
            },
            paramsBinder,
            iterationsOption,
            temperatureOption,
            seedOption
        );
        localSearchCommand.SetHandler(
            (ProblemParams problemParams) =>
            {
                Console.WriteLine("Running local-search solver...");
                RunLSSolver(problemParams);
            },
            paramsBinder
        );

        return new Command("solve", "Solve the drone hub problem")
        {
            solveManualCommand,
            solveFileCommand,
        };
    }

    private static Command CreateGenerateCommand()
    {
        var seedOption = new Option<int>(
            name: "--seed",
            description: "Seed for randomizer",
            getDefaultValue: () => DateTime.Now.Second
        );
        seedOption.AddAlias("-s");

        var volumeOption = new Option<float>(
            $"--volume",
            $"Map size multiplyer. The value of 1 will return a {ProblemParams.DefaultBounds.Maximum.X - ProblemParams.DefaultBounds.Minimum.X}x{ProblemParams.DefaultBounds.Maximum.Y - ProblemParams.DefaultBounds.Minimum.Y} map"
        );
        volumeOption.SetDefaultValue(1f);
        volumeOption.AddAlias("-v");

        var densityOption = new Option<float>("--density", "Density of delivery points");
        densityOption.SetDefaultValue(0.2f);
        densityOption.AddAlias("-d");

        var frequencyOption = new Option<float>("--frequency", "Maximum frequency of deliveries");
        frequencyOption.SetDefaultValue(4f);
        frequencyOption.AddAlias("-f");

        var deviationOption = new Option<float>(
            "--deviation",
            "Deviation factor from target values of field size and number of points"
        );
        deviationOption.SetDefaultValue(0.2f);
        deviationOption.AddAlias("-e");

        var countOption = new Option<uint?>(
            "--count",
            "The exact number of delivery points to generate. If this value is set, the --density option is ignored."
        );
        countOption.AddAlias("-n");

        var sizeXOption = new Option<uint?>(
            "--size-x",
            "The exact size of the field in X coordinates. If this value is set, the --volume option is ignored."
        );
        var sizeYOption = new Option<uint?>(
            "--size-y",
            "The exact size of the field in Y coordinates. Works only in conjunction with the --size-x option."
        );

        var generatorCommand = new Command("generate", "Generate problem parameters")
        {
            volumeOption,
            densityOption,
            frequencyOption,
            deviationOption,
            countOption,
            sizeXOption,
            sizeYOption,
            seedOption,
        };

        generatorCommand.SetHandler(
            (
                float volume,
                float density,
                float frequency,
                float deviation,
                uint? count,
                uint? sizeX,
                uint? sizeY,
                int seed
            ) =>
            {
                RunGenerator(volume, density, frequency, deviation, seed, count, sizeX, sizeY);
            },
            volumeOption,
            densityOption,
            frequencyOption,
            deviationOption,
            countOption,
            sizeXOption,
            sizeYOption,
            seedOption
        );

        return generatorCommand;
    }

    private static void RunSASolver(ProblemParams problem, SimulatedAnnealingParams parameters)
    {
        SimulatedAnnealing algorithm = new(parameters);

        Solver solver = new() { Algorithm = algorithm, Problem = problem };

        if (solver.Solve(out string? error) == false)
        {
            LogError(error);
            return;
        }

        PrintSolution(solver.LastSolution!);
    }

    private static void RunLSSolver(ProblemParams problem)
    {
        LocalSearch algorithm = new();

        Solver solver = new() { Algorithm = algorithm, Problem = problem };

        if (solver.Solve(out string? error) == false)
        {
            LogError(error);
            return;
        }

        PrintSolution(solver.LastSolution!);
    }

    private static void RunGenerator(
        float volume,
        float density,
        float frequency,
        float deviation,
        int seed,
        uint? count,
        uint? sizeX,
        uint? sizeY
    )
    {
        ProblemGenerator generator = new()
        {
            Volume = volume,
            Density = density,
            Frequency = frequency,
            Deviation = deviation,
            Seed = seed,
            Count = count,
        };

        if (sizeX is not null)
            generator.Bounds = new(
                new(0, 0),
                new(
                    Convert.ToInt32(sizeX.Value),
                    Convert.ToInt32(sizeY.HasValue ? sizeY.Value : sizeX.Value)
                )
            );

        var generated = generator.Generate();
        Console.WriteLine(JsonConvert.SerializeObject(generated, Formatting.Indented));
    }

    private static void PrintSolution(ProblemSolution solution)
    {
        Console.WriteLine(
            $"""
            Solution:
              Hub Location:      ({solution.Result.X}, {solution.Result.Y})
              Objective:         {solution.ResultObjective:F6}
              Time took:         {solution.TimeTook}
              Iterations took:   {solution.IterationsTook}
            """
        );
    }

    private static void LogError(string text)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(text);
        Console.ForegroundColor = color;
    }
}
