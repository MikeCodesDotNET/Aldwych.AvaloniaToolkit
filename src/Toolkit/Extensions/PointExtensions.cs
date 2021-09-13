using Avalonia;

namespace Aldwych.AvaloniaToolkit.Extensions
{
    public static class PointExtensions
    {
        public static double LengthSquared(this Point point)
        {
            return point.X * point.X + point.Y * point.Y;
        }

    }
}
