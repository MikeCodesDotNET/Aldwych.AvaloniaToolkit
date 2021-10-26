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
    public class AddWindowViewModel : ViewModelBase
    {
        public Workspace TouchGridLayout { get; set; }

        public AddWindowViewModel()
        {
        }


        public void AddCommandLineHistoryWindow()
        {
            TouchGridLayout?.PresentDocumentView(typeof(CommandLineHistoryView));
        }

        public void AddClockWindow()
        {
            TouchGridLayout?.PresentDocumentView(typeof(ClockView));
        }

    }
}
