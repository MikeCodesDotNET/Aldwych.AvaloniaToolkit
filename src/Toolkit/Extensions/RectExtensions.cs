
using Avalonia;

namespace Aldwych.AvaloniaToolkit.Extensions
{
    public static class RectExtensions
    {
        public static double GetCenterX(this Rect rect)
        {
            return rect.Left + (rect.Width / 2);
        }

        public static double GetCenterY(this Rect rect)
        {
            return rect.Top + (rect.Height / 2);
        }

        /// <summary>
        /// Scales the rectangle proportionately to be no bigger than maxSize, or leaves it alone if it already fits.
        /// </summary>
        /// <returns>Returns the scale factor used.</returns>
        public static double GetProportionalScale(this Rect rect, Size maxSize)
        {
            return System.Linq.Enumerable.Min(new[]
            {
             1.0,
             maxSize.Width / rect.Width,
             maxSize.Height / rect.Height
         });
        }

        public static Point LeftCenterPoint(this Rect bounds)
        {
            Point point = new Point(bounds.Left, bounds.GetCenterY());
            return point;
        }

        public static Point RightCenterPoint(this Rect bounds)
        {
            Point point = new Point(bounds.Right, bounds.GetCenterY());
            return point;
        }

        public static Point TopCenterPoint(this Rect bounds)
        {
            Point point = new Point(bounds.GetCenterX(), bounds.Top);
            return point;
        }

        public static Point BottomCenterPoint(this Rect bounds)
        {
            Point point = new Point(bounds.GetCenterX(), bounds.Bottom);
            return point;
        }

        public static Rect Offset(this Rect rect, double offsetX, double offsetY)
        {
            return new Rect(rect.X + offsetX, rect.Y + offsetY, rect.Width, rect.Height);
        }
    }
}
