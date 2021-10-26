using Aldwych.AvaloniaToolkit.Abstract;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;

using System;
using System.ComponentModel;

namespace Aldwych.Mdi.Controls
{
    public interface IWindow : IHasShow, IHasClose, IHasTitle
    {
        SizeToContent SizeToContent { get; set; }

        Rect Bounds { get; }

        object Content { get; set; }

        HorizontalAlignment HorizontalContentAlignment { get; set; }

        VerticalAlignment VerticalContentAlignment { get; set; }

        bool IsActive { get; }

        bool Topmost { get; set; }

        void Hide();

        void Activate();

        object? Owner { get; }

        bool CanResize { get; set; }

        WindowStartupLocation WindowStartupLocation { get; set; }

        PixelPoint Position { get; set; }

        void BeginMoveDrag(PointerPressedEventArgs e);

        void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e);

        event EventHandler<CancelEventArgs> Closing;
        event EventHandler Activated;
        event EventHandler Deactivated;
        event EventHandler<Point> PositionChanged;
        event EventHandler<Point> PositionChanging;
        event EventHandler Opened;
        event EventHandler Closed;
        event EventHandler LayoutUpdated;

        /// <summary>
        /// Gets or sets the client size of the window.
        /// </summary>
        Size ClientSize { get; }



        void Close(object dialogResult);


    }
}

