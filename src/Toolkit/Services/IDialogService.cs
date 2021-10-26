using Aldwych.AvaloniaToolkit.ViewModels;
using Avalonia.Controls;

namespace Aldwych.AvaloniaToolkit.Services
{
    public interface IDialogService
    {
        void Show(string message);

        void Show(string title, string message);

        void Show(string title, string message, Button[] buttons);

        bool Confirm(string title, string message, string trueText = "Yes", string falseText = "No");

        void Show<T>() where T: ViewModelBase;
    }
}
