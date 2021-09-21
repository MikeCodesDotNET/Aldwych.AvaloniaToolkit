using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Linq;


namespace Aldwych.AvaloniaToolkit.Controls
{
    public class Rate : UserControl
    {
        public static readonly StyledProperty<bool> ClearEnabledProperty = AvaloniaProperty.Register<Rate, bool>(nameof(ClearEnabled));

        public static readonly StyledProperty<StreamGeometry> IconProperty = AvaloniaProperty.Register<Rate, StreamGeometry>(nameof(Icon));

        public static readonly StyledProperty<int> DefaultValueProperty = AvaloniaProperty.Register<Rate, int>(nameof(DefaultValue));

        public static readonly StyledProperty<int> ValueProperty = AvaloniaProperty.Register<Rate, int>(nameof(Value));

        public static readonly StyledProperty<int> MaxValueProperty = AvaloniaProperty.Register<Rate, int>(nameof(MaxValue), 5);

        public static readonly StyledProperty<IBrush> SelectedColorProperty = AvaloniaProperty.Register<Rate, IBrush>(nameof(SelectedColor));

        public static readonly StyledProperty<IBrush> DefaultColorProperty = AvaloniaProperty.Register<Rate, IBrush>(nameof(DefaultColor));

        public IBrush DefaultColor
        {
            get { return GetValue(DefaultColorProperty); }
            set { SetValue(DefaultColorProperty, value); }
        }
        public IBrush SelectedColor
        {
            get { return GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }


        public Rate()
        {
            this.Content = container;
            this.PropertyChanged += Rate_PropertyChanged;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;

            MinWidth = 200;
            MinHeight = 25;

            this.PointerPressed += Rate_PointerPressed;
        }


        private void Rate_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (e.KeyModifiers == KeyModifiers.Control && ClearEnabled)
            {
                Value = 0;
                return;
            }

            var itemWithPointerOver = container.Children.Where(x => x.IsPointerOver).FirstOrDefault();
            if (itemWithPointerOver == null)
                return;

            var index = container.Children.IndexOf(itemWithPointerOver);
            Value = index + 1;
        }

        private void Rate_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == MaxValueProperty)
            {
                if (Icon == null)
                    return;

                SyncContainerContentCount();
            }

            if (e.Property == ValueProperty)
            {
                if (Icon == null)
                    return;

                if (!container.Children.Any())
                    SyncContainerContentCount();

                UpdateSelected();
            }

            if (e.Property == IconProperty)
            {
                SyncContainerContentCount();
                UpdateSelected();
            }

            if (e.Property == HeightProperty)
                if (container != null) container.Height = this.Height;
        }

        public int MaxValue
        {
            get { return GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public int Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public int DefaultValue
        {
            get { return GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }

        public StreamGeometry Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public bool ClearEnabled
        {
            get { return GetValue(ClearEnabledProperty); }
            set { SetValue(ClearEnabledProperty, value); }
        }

        private void UpdateSelected()
        {
            if (Value > MaxValue)
                return;

            var selected = container.Children.GetRange(0, Value);

            foreach (Path s in container.Children)
            {
                if (selected.Contains(s))
                    s.Fill = SelectedColor;
                else
                    s.Fill = DefaultColor;
            }
        }

        private void SyncContainerContentCount()
        {
            var cc = container.Children.Count;

            if (MaxValue == cc)
                return;

            if (MaxValue < cc)
            {
                var diff = cc - MaxValue;
                for (int i = 0; i < diff; i++)
                {
                    container.Children.RemoveAt(cc - i);
                }
            }
            else
            {
                var diff = MaxValue - cc;
                for (int i = 0; i < diff; i++)
                {
                    var ic = new Path() { Data = Icon, Fill = DefaultColor, MinHeight = 16, VerticalAlignment = VerticalAlignment.Stretch, Stretch = Stretch.UniformToFill };
                    ic.PointerPressed += Rate_PointerPressed;
                    container.Children.Add(ic);
                }
            }
        }

        private StackPanel container { get; } = new StackPanel()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Orientation = Orientation.Horizontal,
            Spacing = 8
        };
    }
}
