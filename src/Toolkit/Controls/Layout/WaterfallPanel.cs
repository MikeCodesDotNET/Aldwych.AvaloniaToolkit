using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

using System.Linq;

namespace Aldwych.AvaloniaToolkit.Controls
{
    public class WaterfallPanel : Panel
    {
        public static readonly StyledProperty<int> GroupsProperty = AvaloniaProperty.Register<WaterfallPanel, int>(nameof(Groups));

        public static readonly StyledProperty<Orientation> OrientationProperty = AvaloniaProperty.Register<WaterfallPanel, Orientation>(nameof(Orientation));

        static WaterfallPanel()
        {
            AffectsMeasure<WaterfallPanel>(GroupsProperty);
            AffectsMeasure<WaterfallPanel>(OrientationProperty);
        }

        public Orientation Orientation
        {
            get { return GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public int Groups
        {
            get { return GetValue(GroupsProperty); }
            set { SetValue(GroupsProperty, value); }
        }

        protected override Size ArrangeOverride(Size availableSize)
        {
            var groups = Groups;

            if (groups < 1) return availableSize;

            var children = Children;
            Size panelSize;

            if (Orientation == Orientation.Horizontal)
            {
                var heightArr = new double[groups].ToList();
                var itemWidth = availableSize.Width / groups;
                if (double.IsNaN(itemWidth) || double.IsInfinity(itemWidth)) return availableSize;

                for (int i = 0, count = children.Count(); i < count; i++)
                {
                    var child = children[i];
                    if (child == null) continue;

                    child.Measure(availableSize);
                    var minIndex = heightArr.IndexOf(heightArr.Min());
                    var minY = heightArr[minIndex];

                    var size = new Size(itemWidth, child.DesiredSize.Height);
                    var point = new Point(minIndex * itemWidth, minY);
                    var rect = new Rect(point, size);
                    child.Arrange(rect);

                    heightArr[minIndex] = minY + child.DesiredSize.Height;
                }
                panelSize = new Size(availableSize.Width, heightArr.Max());
            }
            else
            {
                var widthArr = new double[groups].ToList();
                var itemHeight = availableSize.Height / groups;
                if (double.IsNaN(itemHeight) || double.IsInfinity(itemHeight)) return availableSize;

                for (int i = 0, count = children.Count(); i < count; i++)
                {
                    var child = children[i];
                    if (child == null) continue;

                    child.Measure(availableSize);
                    var minIndex = widthArr.IndexOf(widthArr.Min());
                    var minX = widthArr[minIndex];
                    child.Arrange(new Rect(new Point(minX, minIndex * itemHeight), new Size(child.DesiredSize.Width, itemHeight)));

                    widthArr[minIndex] = minX + child.DesiredSize.Width;
                }
                panelSize = new Size(widthArr.Max(), availableSize.Height);
            }

            return panelSize;
        }
    }
}
