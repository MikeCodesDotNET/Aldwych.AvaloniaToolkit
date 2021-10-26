using Aldwych.Mdi.Controls;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SampleApp.ViewModels;

namespace SampleApp.Views
{
    public class AddWindowView : UserControl, IAddWindowProvider
    {

        AddWindowViewModel viewModel; 

        public AddWindowView()
        {
            this.InitializeComponent();
            viewModel = new AddWindowViewModel();
            this.DataContext = viewModel;
        }

        Workspace touchGridLayout;
        public Workspace TargetLayout
        {
            get => touchGridLayout;
            set
            {
                touchGridLayout = value;
                viewModel.TouchGridLayout = value;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
