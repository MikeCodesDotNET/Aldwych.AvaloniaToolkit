using Aldwych.AvaloniaToolkit.ViewModels;
using Aldwych.Mdi.Controls;
using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace Aldwych.Mdi.Services
{
    public interface IWindowManagementService
    {
        IWindow Show(Window window);

        IWindow MainWindow { get; }

        void Show(IWindow window);

        void Show<T>() where T : ViewModelBase;

        void Show<T>(T viewModel) where T : ViewModelBase;

        void CloseWindow(IWindow window);

        void CloseAll(bool skipMainWindow = true);

        IWindow ShowDialog(Window window);

        void ShowDialog(IWindow window);

        IReadOnlyList<IWindow> Windows { get; }

        event EventHandler<WindowEventArgs> WindowClosed;
        event EventHandler<WindowEventArgs> WindowShown;
    }

    public class WindowEventArgs : EventArgs
    {
        public IWindow Window;

        public WindowEventArgs(IWindow window)
        {
            Window = window;
        }
    }
}
