using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Aldwych.AvaloniaToolkit.Controls
{
    public class CirclePanel : Panel
    {

        public static readonly StyledProperty<double> DiameterProperty = AvaloniaProperty.Register<CirclePanel, double>(nameof(Diameter), 170.0);

        public static readonly StyledProperty<bool> KeepVerticalProperty = AvaloniaProperty.Register<CirclePanel, bool>(nameof(KeepVertical), false);


        public static readonly StyledProperty<double> OffsetAngleProperty = AvaloniaProperty.Register<CirclePanel, double>(nameof(OffsetAngle), 0);


        static CirclePanel()
        {
            AffectsMeasure<CirclePanel>(DiameterProperty);
            AffectsMeasure<CirclePanel>(KeepVerticalProperty);
            AffectsMeasure<CirclePanel>(OffsetAngleProperty);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var diameter = Diameter;

            if (Children.Count == 0) return new Size(diameter, diameter);

            var newSize = new Size(diameter, diameter);

            foreach (IControl element in Children)
            {
                element.Measure(newSize);
            }

            return newSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var keepVertical = KeepVertical;
            var offsetAngle = OffsetAngle;

            var i = 0;
            var perDeg = 360.0 / Children.Count;
            var radius = Diameter / 2;

            foreach (IControl element in Children)
            {
                var centerX = element.DesiredSize.Width / 2.0;
                var centerY = element.DesiredSize.Height / 2.0;
                var angle = perDeg * i++ + offsetAngle;

                var transform = new RotateTransform
                {
                    Angle = keepVertical ? 0 : angle
                };
                element.RenderTransformOrigin = RelativePoint.Center;
                element.RenderTransform = transform;

                var r = Math.PI * angle / 180.0;
                var x = radius * Math.Cos(r);
                var y = radius * Math.Sin(r);

                var rectX = x + finalSize.Width / 2 - centerX;
                var rectY = y + finalSize.Height / 2 - centerY;

                element.Arrange(new Rect(rectX, rectY, element.DesiredSize.Width, element.DesiredSize.Height));
            }

            return finalSize;
        }


        public double OffsetAngle
        {
            get { return GetValue(OffsetAngleProperty); }
            set { SetValue(OffsetAngleProperty, value); }
        }

        public bool KeepVertical
        {
            get { return GetValue(KeepVerticalProperty); }
            set { SetValue(KeepVerticalProperty, value); }
        }

        public double Diameter
        {
            get { return GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }
    }
}
