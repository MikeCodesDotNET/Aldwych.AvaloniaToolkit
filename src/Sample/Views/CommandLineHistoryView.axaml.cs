using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SampleApp.ViewModels;

namespace SampleApp.Views
{
    public class CommandLineHistoryView : UserControl
    {
        public CommandLineHistoryView()
        {
            this.InitializeComponent();
            this.DataContext = new CommandLineHistoryViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
