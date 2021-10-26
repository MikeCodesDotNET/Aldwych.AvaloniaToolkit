using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SampleApp.Views
{
    public partial class ClockView : UserControl
    {
        public ClockView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
