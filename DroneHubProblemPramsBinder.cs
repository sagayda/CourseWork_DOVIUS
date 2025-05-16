using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using CourseWork.DroneHub;
using Newtonsoft.Json;

namespace CourseWork;

public class DroneHubProblemPramsBinder : BinderBase<ProblemParams>
{
    private readonly Option<int[]> _xArrayOption;
    private readonly Option<int[]> _yArrayOption;
    private readonly Option<uint[]> _wArrayOption;
    private readonly Option<int> _minXOption;
    private readonly Option<int> _minYOption;
    private readonly Option<int> _maxXOption;
    private readonly Option<int> _maxYOption;
    private readonly Option<float> _droneDistanceOption;

    private readonly Argument<FileInfo> _filePathOption;

    // private readonly Option<float> _generatorAlphaOption;

    public DroneHubProblemPramsBinder(
        Option<int[]> xArrayOption,
        Option<int[]> yArrayOption,
        Option<uint[]> wArrayOption,
        Option<int> minXOption,
        Option<int> minYOption,
        Option<int> maxXOption,
        Option<int> maxYOption,
        Option<float> droneDistanceOption,
        Argument<FileInfo> filePathOption
    // Option<float> generatorAlphaOption
    )
    {
        _xArrayOption = xArrayOption;
        _yArrayOption = yArrayOption;
        _wArrayOption = wArrayOption;
        _minXOption = minXOption;
        _minYOption = minYOption;
        _maxXOption = maxXOption;
        _maxYOption = maxYOption;
        _droneDistanceOption = droneDistanceOption;

        _filePathOption = filePathOption;

        // _generatorAlphaOption = generatorAlphaOption;
    }

    protected override ProblemParams GetBoundValue(BindingContext bindingContext)
    {
        var parseResult = bindingContext.ParseResult;
        DataSourceType sourceType = DataSourceType.None;

        var currentResult = parseResult.CommandResult;
        while (currentResult is not null && sourceType == DataSourceType.None)
        {
            // Console.WriteLine(currentCommand.Name);
            // currentResult = currentResult.Parents.OfType<Command>().FirstOrDefault();
            currentResult = currentResult.Parent as CommandResult;

            switch (currentResult?.Command.Name)
            {
                case "manual":
                    sourceType = DataSourceType.Manual;
                    break;
                case "file":
                    sourceType = DataSourceType.File;
                    break;
                // case "generator":
                //     sourceType = DataSourceType.Generator;
                //     break;
                default:
                    break;
            }
        }

        ProblemParams? problemParams = null;
        switch (sourceType)
        {
            case DataSourceType.Manual:
                problemParams = ParseManual(parseResult);
                break;
            case DataSourceType.File:
                problemParams = ParseFile(parseResult);
                break;
            // case DataSourceType.Generator:
            //     problemParams = ParseGenerator(parseResult);
            //     break;
            case DataSourceType.None:
            default:
                throw new InvalidOperationException("Failed to determine data source.");
        }

        return problemParams;
    }

    private ProblemParams ParseManual(ParseResult parseResult)
    {
        var x = parseResult.GetValueForOption(_xArrayOption) ?? [];
        var y = parseResult.GetValueForOption(_yArrayOption) ?? [];
        var w = parseResult.GetValueForOption(_wArrayOption) ?? [];

        var items = Math.Min(x.Length, Math.Min(y.Length, w.Length));
        DeliveryPoint[] points = new DeliveryPoint[items];

        for (int i = 0; i < items; i++)
            points[i] = new(new(x[i], y[i]), w[i]);

        IntBounds bounds = new(
            new(
                parseResult.GetValueForOption(_minXOption),
                parseResult.GetValueForOption(_minYOption)
            ),
            new(
                parseResult.GetValueForOption(_maxXOption),
                parseResult.GetValueForOption(_maxYOption)
            )
        );

        float droneDistance = parseResult.GetValueForOption(_droneDistanceOption);

        return new ProblemParams(points, bounds, droneDistance);
    }

    private ProblemParams ParseFile(ParseResult parseResult)
    {
        var file = parseResult.GetValueForArgument(_filePathOption);

        if (file is null)
            throw new InvalidDataException("TODO 1");

        ProblemParams? problemParams = null;
        try
        {
            using var fileReader = file.OpenText();
            using JsonTextReader reader = new(fileReader);

            JsonSerializer serializer = new();
            problemParams = serializer.Deserialize<ProblemParams>(reader);
        }
        catch (System.Exception)
        {
            throw new InvalidDataException("TODO 2");
        }

        if (problemParams is null)
        {
            throw new InvalidDataException("TODO 3");
        }

        return problemParams;
    }

    private enum DataSourceType
    {
        None,
        Manual,
        File,
    }
}
