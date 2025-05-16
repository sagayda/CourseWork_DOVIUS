namespace CourseWork.DroneHub
{
    public class ProblemGenerator
    {
        private Random _workingRandom = new Random(0);

        public float Volume { get; set; } = 1f;
        public float Density { get; set; } = 0.2f;
        public float Frequency { get; set; } = 4f;

        public float Deviation { get; set; } = 0.2f;

        public IntBounds? Bounds { get; set; } = null;
        public uint? Count { get; set; } = null;

        public int Seed
        {
            set { _workingRandom = new Random(value); }
        }

        public ProblemParams Generate()
        {
            IntBounds bounds;
            if (Bounds is not null)
            {
                bounds = Bounds.Value;
            }
            else
            {
                int defaultWidth =
                    ProblemParams.DefaultBounds.Maximum.X - ProblemParams.DefaultBounds.Minimum.X;
                int defaultHeight =
                    ProblemParams.DefaultBounds.Maximum.Y - ProblemParams.DefaultBounds.Minimum.Y;

                float targetWidth = defaultWidth * Volume;
                float targetHeight = defaultHeight * Volume;

                int minSizeX = Math.Max(1, Convert.ToInt32(targetWidth * (1f - Deviation)));
                int maxSizeX = Math.Max(minSizeX, Convert.ToInt32(targetWidth * (1f + Deviation)));

                int sizeX = _workingRandom.Next(minSizeX, maxSizeX + 1);

                int minSizeY = Math.Max(1, Convert.ToInt32(targetHeight * (1f - Deviation)));
                int maxSizeY = Math.Max(minSizeY, Convert.ToInt32(targetHeight * (1f + Deviation)));

                int sizeY = _workingRandom.Next(minSizeY, maxSizeY + 1);

                bounds = new(new(0, 0), new(sizeX, sizeY));
            }

            uint count;
            if (Count is not null)
            {
                count = Count.Value;
            }
            else
            {
                int width = bounds.Maximum.X - bounds.Minimum.X + 1;
                int height = bounds.Maximum.Y - bounds.Minimum.Y + 1;

                long maxPossiblePoints = (long)Math.Max(width, 0) * Math.Max(height, 0);

                float targetCountFloat = maxPossiblePoints * Math.Min(0.9f, Density);

                int minCount = Math.Max(1, Convert.ToInt32(targetCountFloat * (1f - Deviation)));
                int actualTargetCount = Math.Max(1, Convert.ToInt32(targetCountFloat));

                if (minCount > actualTargetCount)
                    minCount = actualTargetCount;

                if (maxPossiblePoints == 0 || actualTargetCount == 0)
                {
                    count = 0;
                }
                else
                {
                    count = Convert.ToUInt32(_workingRandom.Next(minCount, actualTargetCount + 1));
                }
            }

            HashSet<IntPoint> uniquePoints = new HashSet<IntPoint>();
            while (uniquePoints.Count < count)
            {
                IntPoint newPoint = RandomPointInBounds(bounds);
                uniquePoints.Add(newPoint);
            }

            IntPoint[] points = uniquePoints.ToArray();

            DeliveryPoint[] deliveryPoints = new DeliveryPoint[count];
            for (uint i = 0; i < count; i++)
            {
                uint deliveries = Convert.ToUInt32(
                    Math.Max(_workingRandom.NextSingle() * Frequency, 1)
                );

                deliveryPoints[i] = new(points[i], deliveries);
            }

            int greaterSide = Math.Max(
                bounds.Maximum.X - bounds.Minimum.X + 1,
                bounds.Maximum.Y - bounds.Minimum.Y + 1
            );

            float droneDistance = (_workingRandom.NextSingle() * 0.6f + 0.2f) * greaterSide;

            return new(deliveryPoints, bounds, droneDistance);
        }

        private IntPoint RandomPointInBounds(IntBounds bounds)
        {
            return new IntPoint(
                _workingRandom.Next(bounds.Minimum.X, bounds.Maximum.X + 1),
                _workingRandom.Next(bounds.Minimum.Y, bounds.Maximum.Y + 1)
            );
        }
    }
}
