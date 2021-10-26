using Aldwych.Mdi.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using System;
using System.ComponentModel;

namespace Aldwych.Mdi.Controls
{
    public class WindowWrapper : IWindow
    {
        private readonly Window _window;

        public WindowWrapper(Window window)
        {
            _window = window;
            _window.Closing += (s, e) => Closing?.Invoke(this, e);
            _window.Activated += (s, e) => Activated?.Invoke(this, e);
            _window.Deactivated += (s, e) => Deactivated?.Invoke(this, e);
            _window.PositionChanged += (s, e) => PositionChanged?.Invoke(this, new Point(_window.Bounds.X, _window.Bounds.Y));
            _window.Opened += (s, e) => Opened?.Invoke(this, e);
            _window.Closed += (s, e) => Closed?.Invoke(this, e);
            _window.LayoutUpdated += (s, e) => LayoutUpdated?.Invoke(this, e);
            _window.GotFocus += (s, e) => GotFocus?.Invoke(this, e);
            _window.LostFocus += (s, e) => GotFocus?.Invoke(this, e);

        }

        public Rect Bounds { get => _window.Bounds;  }

        public object Owner { get => _window.Owner; }

        public SizeToContent SizeToContent { get => _window.SizeToContent; set => _window.SizeToContent = value; }

        public string Title { get => _window.Title; set => _window.Title = value; }

        public bool IsActive { get => _window.IsActive; }

        public bool Topmost { get => _window.Topmost; set => _window.Topmost = value; }

        public bool CanResize { get => _window.CanResize; set => _window.CanResize = value; }

        public object? DataContext { get => _window.DataContext; set => _window.DataContext = value; }

        public WindowStartupLocation WindowStartupLocation { get => _window.WindowStartupLocation; set => _window.WindowStartupLocation = value; }

        public PixelPoint Position { get => _window.Position; set => _window.Position = value; }

        public Size ClientSize => _window.ClientSize;

        public object Content { get => _window.Content; set => _window.Content = value; }

        public HorizontalAlignment HorizontalContentAlignment { get => _window.HorizontalContentAlignment; set => _window.HorizontalContentAlignment = value; }

        public VerticalAlignment VerticalContentAlignment { get => _window.VerticalContentAlignment; set => _window.VerticalContentAlignment = value; }


        public event EventHandler<CancelEventArgs> Closing;
        public event EventHandler Activated;
        public event EventHandler Deactivated;
        public event EventHandler<Point> PositionChanged;
        public event EventHandler<Point> PositionChanging;
        public event EventHandler Opened;
        public event EventHandler Closed;
        public event EventHandler LayoutUpdated;
        public event EventHandler GotFocus;
        public event EventHandler LostFocus;


        public void Activate() => _window.Activate();

        public void BeginMoveDrag(PointerPressedEventArgs e) => _window.BeginMoveDrag(e);

        public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e) => _window.BeginResizeDrag(edge, e);

        public void Close() => _window.Close();

        public void Close(object dialogResult) => _window.Close(dialogResult);

        public void Hide() => _window.Hide();

        public void Show() => _window.Show();
    }
}
