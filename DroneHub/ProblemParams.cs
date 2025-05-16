using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CourseWork.DroneHub;

public record class ProblemParams
{
    public static IntBounds DefaultBounds = new(new(0, 0), new(64, 64));
    public static double DefaultDroneDistance = 32d;

    public DeliveryPoint[] Points { get; set; }

    public IntBounds Bounds { get; set; } = DefaultBounds;
    public double DroneDistance { get; set; } = DefaultDroneDistance;

    public ProblemParams(DeliveryPoint[] points, IntBounds bounds, double droneDistance)
    {
        Points = points;
        Bounds = bounds;
        DroneDistance = droneDistance;
    }

    public double CalculateObjectiveFor(IntPoint hubCoordinates)
    {
        double result = 0d;

        foreach (var item in Points)
        {
            var distance = hubCoordinates.DistanceTo(item.Coordinates);

            if (distance <= DroneDistance / 2d && distance > 0)
            {
                result += item.Deliveries / distance;
            }
        }

        return result;
    }

    public bool Validate([NotNullWhen(false)] out string? error)
    {
        error = null;

        if (Bounds.Maximum.X < Bounds.Minimum.X || Bounds.Maximum.Y < Bounds.Minimum.Y)
        {
            error = "The minimum value of the boundaries cannot be greater than the maximum";
            return false;
        }

        foreach (var point in Points)
        {
            if (Bounds.Contains(point.Coordinates) == false)
            {
                error = $"Delivery point ({point.Coordinates}) is outside the bounds";
                return false;
            }
        }

        if (DroneDistance <= 0)
        {
            error = "Drone distance must be greater than zero\n";
            return false;
        }

        return true;
    }
}
