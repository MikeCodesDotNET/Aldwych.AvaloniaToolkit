using Aldwych.AvaloniaToolkit.Navigation;
using Aldwych.AvaloniaToolkit.ViewModels;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampleApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public MainWindowViewModel()
        {            
        }

        public void SetContentRed()
        {
            PushViewModel<DummyViewModel>("MainContent");
        }

        public void SetContentGreen()
        {
            PushViewModel<DummyViewModel>(vm => vm.Title = "Green", "MainContent", TransitionType.Slide);
        }

        public void SetContentBlue()
        {
            PushViewModel<DummyViewModel>(vm => vm.Title = "Blue", "MainContent", TransitionType.Crossfade);
        }
        
    }
}
