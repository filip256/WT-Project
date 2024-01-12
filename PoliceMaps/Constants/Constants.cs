namespace PoliceMaps.Constants
{
    public static class Constants
    {
        public static readonly int PlaceConfidenceLevelMultiplier = 5;
        public static readonly int PlaceConfidenceLikeMultiplier = 3;
        public static readonly int MinPlaceConfidence = 0;

        public static readonly int PlaceSeverityIncrement = 1;
        public static readonly TimeSpan MinPlaceReocurranceTime = TimeSpan.FromHours(3);
        public static readonly double MaxSpatialEqualityDistance = 0.000006;

        public static readonly int WazeRefreshRateBaseMins = 10;
        public static readonly int WazeRefreshRateMaxRandomMins = 2;
    }
}
