using Aldwych.AvaloniaToolkit.ViewModels;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampleApp.ViewModels
{
    public class CommandLineHistoryViewModel : ClosableViewModelBase
    {
        public string LogText { get; set; }

        public IBrush BackgroundColor { get; private set; }


        public CommandLineHistoryViewModel() : base(null)
        {
            var bgs = new List<IBrush>() { Brushes.AliceBlue, Brushes.Aqua, Brushes.Aquamarine, Brushes.Azure, Brushes.Blue, Brushes.Cyan, Brushes.DarkBlue, Brushes.DarkGreen, Brushes.Khaki, Brushes.LightBlue, Brushes.Green, Brushes.Magenta, Brushes.Purple, Brushes.Salmon };
            var rnd = new Random();
            var i = rnd.Next(0, bgs.Count);
            BackgroundColor = bgs[i];
        }
    }
}
