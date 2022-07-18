using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using System;

namespace Aldwych.Navigation
{
    public static class Region
    {
        public static readonly AvaloniaProperty<string> RegionNameProperty =
           AvaloniaProperty.RegisterAttached<AvaloniaObject, string>("RegionName", typeof(Region), string.Empty, true, BindingMode.TwoWay);


        static Region()
        {
            RegionNameProperty.Changed.Subscribe(args => {
                if(args.Sender.GetType().IsAssignableFrom(typeof(ContentControl)))
                {
                    var contentControl = args.Sender as ContentControl;

                    contentControl.Tag = args.NewValue.Value;

                    contentControl.AttachedToLogicalTree += (s, e) =>
                    {

                    };

                    contentControl.DetachedFromLogicalTree += (s, e) =>
                    {

                    };

                    RegionManager.Instance.RegisterRegion(args.NewValue.Value, contentControl);

                }
            });
        }

        public static void SetRegionName(AvaloniaObject element, string value)
        {
            element.SetValue(RegionNameProperty, value);
        }

        public static string GetRegionName(AvaloniaObject element)
        {
            return (string)element.GetValue(RegionNameProperty);
        }
    }
}
