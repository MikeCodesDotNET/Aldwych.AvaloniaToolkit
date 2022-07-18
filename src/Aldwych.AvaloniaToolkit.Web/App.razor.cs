using Avalonia.ReactiveUI;
using Avalonia.Web.Blazor;

namespace Aldwych.AvaloniaToolkit.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        WebAppBuilder.Configure<SampleApp.App>()
            .UseReactiveUI()
            .SetupWithSingleViewLifetime();
    }
}