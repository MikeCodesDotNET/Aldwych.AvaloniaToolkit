using Aldwych.AvaloniaToolkit.Controls;
using Aldwych.AvaloniaToolkit.ViewModels;
using Aldwych.Mdi.Controls;
using SampleApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApp.ViewModels
{
    public class AddWindowViewModel : ClosableViewModelBase
    {
        public Workspace TouchGridLayout { get; set; }

        public AddWindowViewModel() : base(null)
        {
        }


        public void AddCommandLineHistoryWindow()
        {
            TouchGridLayout?.Show(new CommandLineHistoryViewModel());
        }

        public void AddClockWindow()
        {
            TouchGridLayout?.Show(new ClockViewModel(), WindowType.Alert);
        }

    }
}
