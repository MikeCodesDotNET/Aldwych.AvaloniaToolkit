using Avalonia;

namespace Aldwych.AvaloniaToolkit.Extensions
{
	public static class SizeExtensions
	{
		public static Size Subtract(this Size size, Size amount)
		{
			var w = size.Width - amount.Width;
			var h = size.Height - amount.Height;
			return new Avalonia.Size(w, h);
		}

		public static Size Subtract(this Size size, Point amount)
		{
			var w = size.Width - amount.X;
			var h = size.Height - amount.Y;
			return new Avalonia.Size(w, h);
		}

		public static Size Add(this Size size, Size amount)
		{
			var w = size.Width + amount.Width;
			var h = size.Height + amount.Height;
			return new Avalonia.Size(w, h);
		}
	}
}
