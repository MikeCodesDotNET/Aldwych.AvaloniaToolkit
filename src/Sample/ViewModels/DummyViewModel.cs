using Aldwych.AvaloniaToolkit.ViewModels;
using ReactiveUI;

namespace SampleApp.ViewModels
{
    public class DummyViewModel : ViewModelBase
    {

        private string _title;
        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }


        public DummyViewModel()
        {
            Title = "Empty";
        }
    }
}
