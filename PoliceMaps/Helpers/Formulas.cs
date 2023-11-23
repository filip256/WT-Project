namespace PoliceMaps.Helpers
{
    public static class Formulas
    {
        public static double SquaredDistance(double xA, double yA, double xB, double yB)
        {
            return Math.Abs((xA - xB) * (xA - xB) + (yA - yB) * (yA - yB));
        }
    }
}
