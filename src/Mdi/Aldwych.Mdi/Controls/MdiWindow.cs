using Aldwych.AvaloniaToolkit.Extensions;
using Aldwych.AvaloniaToolkit.ViewModels;
using Aldwych.Mdi.Helpers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.VisualTree;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Aldwych.Mdi.Controls
{
    public class MdiWindow : HeaderedContentControl, IWindow
    {

        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<Workspace, string>(nameof(Title), string.Empty);


        public static readonly StyledProperty<bool> OptionsButtonIsVisibleProperty =
            AvaloniaProperty.Register<Workspace, bool>(nameof(OptionsButtonIsVisible), false);


        public static readonly StyledProperty<bool> IsResizableProperty =
            AvaloniaProperty.Register<Workspace, bool>(nameof(IsResizable), false);


        public static readonly StyledProperty<List<IControl>> ToolbarItemsProperty =
            AvaloniaProperty.Register<Workspace, List<IControl>>(nameof(ToolbarItems), new List<IControl>());


        public static readonly StyledProperty<double> ToolbarHeightProperty =
            AvaloniaProperty.Register<Workspace, double>(nameof(ToolbarHeight), 50);


        public static readonly StyledProperty<StartupLocation> StartupLocationProperty =
            AvaloniaProperty.Register<Workspace, StartupLocation>(nameof(StartupLocation), StartupLocation.CenterParent);


        public static readonly StyledProperty<Point> PositionProperty =
            AvaloniaProperty.Register<MdiWindow, Point>(nameof(Position));

        public static readonly StyledProperty<Size> SizeProperty = 
            AvaloniaProperty.Register<MdiWindow, Size>(nameof(Size));

        public static new readonly StyledProperty<ClosableViewModelBase> ContentProperty =
           AvaloniaProperty.Register<MdiWindow, ClosableViewModelBase>(nameof(Content));

        public static readonly StyledProperty<WindowType> WindowTypeProperty =
         AvaloniaProperty.Register<MdiWindow, WindowType>(nameof(WindowType), WindowType.UserView);

        public WindowType WindowType
        {
            get { return GetValue(WindowTypeProperty); }
            set
            {
                SetValue(WindowTypeProperty, value);
                base.Content = value;
            }
        }

        public new ClosableViewModelBase Content
        {
            get { return GetValue(ContentProperty); }
            set 
            { 
                SetValue(ContentProperty, value);
                base.Content = value;
            }
        }

        public static readonly StyledProperty<bool> IsActiveProperty = AvaloniaProperty.Register<MdiWindow, bool>(nameof(IsActive));

   

        public bool IsActive
        {
            get { return GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly StyledProperty<Brush> ActiveBorderBrushProperty = 
            AvaloniaProperty.Register<MdiWindow, Brush>(nameof(ActiveBorderBrush));



        public Brush ActiveBorderBrush
        {
            get { return GetValue(ActiveBorderBrushProperty); }
            set { SetValue(ActiveBorderBrushProperty, value); }
        }


        public static readonly StyledProperty<Workspace> WorkspaceProperty = AvaloniaProperty.Register<MdiWindow, Workspace>(nameof(Workspace), null);

        public Workspace Workspace
        {
            get { return GetValue(WorkspaceProperty); }
            set { SetValue(WorkspaceProperty, value); }
        }


        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == WorkspaceProperty && !change.NewValue.HasValue)
            {
                var pc = this.GetVisualParent<Workspace>();
                if (pc != null)
                    Workspace = pc;
                else
                    Workspace = new Workspace();
            }


            if (change.Property == IsActiveProperty)
            {
                if (change.NewValue.GetValueOrDefault<bool>())
                {
                    BorderBrush = defaultBorderBrush;
                }
                else
                {
                    BorderBrush = ActiveBorderBrush;
                }
            }
        }

        //Workspace parentLayout => this.GetVisualParent<Workspace>();

        bool opened;
        protected override void ArrangeCore(Rect finalRect)
        {
            base.ArrangeCore(finalRect);

            if (opened)
                return;

            switch (StartupLocation)
            {
                case StartupLocation.TopLeft:
                    this.Position = new Point(0, 0);
                    break;

                case StartupLocation.BottomLeft:
                    Position = new Point(0, Workspace.Height - this.Height);
                    break;

                case StartupLocation.TopRight:
                    Position = new Point(Workspace.Width - this.Width, 0);
                    break;

                case StartupLocation.BottomRight:
                    Position = new Point(Workspace.Width - this.Width, Workspace.Height - this.Height);
                    break;

                case StartupLocation.CenterParent:
                    Position = new Point(Workspace.Bounds.Center.X - (this.Bounds.Size.Width / 2), Workspace.Bounds.Center.Y - (this.Bounds.Size.Height / 2));
                    break;
                case StartupLocation.Manual:
                    Size = CalculateFillRect(Position).Size;
                    break;
            }
            opened = true;
        }


        /// <summary>
        /// This is pretty nasty. Ideally need to re-work this 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Rect CalculateFillRect(Point p)
        {
            IEnumerable<Rect> rectangles = Workspace.Children.Where(x => x.GetType() == this.GetType()).Where(x => x != this).Select(X => X.Bounds);
            

            //var rectangles = parentLayout.Children.OfType<DocumentViewContainer>().Where(x => x != this).Select(x => x.Bounds);
            var root = Workspace.Bounds;
            if (rectangles.Count() == 0)
                return new Rect(p, root.Size.Subtract(p));

            Rect? vBound = null;
            Rect? hBound = null;

            double height = 0;
            double width = 0;

            //Height
            foreach (var item in rectangles)
            {
                var rect = item;
                if (rect.X + rect.Width <= p.X || rect.Y + rect.Height <= p.Y)
                {
                    continue;
                }

                if (!vBound.HasValue)
                {
                    vBound = item;
                }
                else
                {
                    if (rect.Y < vBound.Value.Y || rect.Y + rect.Height <= vBound.Value.Y)
                    {
                        //rect is higher than vbound 

                        if (rect.X > (vBound.Value.X + vBound.Value.Width))
                        {
                            hBound = item;
                        }
                        else
                        {
                            vBound = item;
                        }
                    }
                }

            }

            if (!hBound.HasValue)
            {
                hBound = vBound;
                vBound = null;
            }


            if (hBound.HasValue)
                width = hBound.Value.X - p.X;
            else
                width = root.Width - p.X;

            if (vBound.HasValue)
                height = vBound.Value.Y - p.Y;
            else
                height = root.Height - p.Y;


            return new Rect(p, new Avalonia.Size(width, height));
        }


        Panel toolbarBackground;
        StackPanel toolbarControls;
        Button closeButton;
        Button optionsButton;
        Canvas dragger;

        IBrush defaultBorderBrush;

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            AddHandler(PointerPressedEvent, PressedHandler, RoutingStrategies.Tunnel);
        }

        private void PressedHandler(object? sender, PointerPressedEventArgs e)
        {
            Workspace.ActiveWindow = this;
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            closeButton = e.NameScope.Find<Button>("PART_CloseButton");
            closeButton.Click += CloseButton_Click;

            defaultBorderBrush = BorderBrush.ToImmutable();


            dragger = e.NameScope.Find<Canvas>("PART_ResizeDragger");
            dragger.PointerPressed += Dragger_PointerPressed;
            dragger.PointerMoved += Dragger_PointerMoved;
            dragger.PointerReleased += Dragger_PointerReleased;

            toolbarBackground = e.NameScope.Find<Panel>("PART_TitleBar");
            if (toolbarBackground != null)
            {
                toolbarBackground.PointerPressed += ToolbarBackground_PointerPressed;
                toolbarBackground.PointerMoved += ToolbarBackground_PointerMoved;
                toolbarBackground.PointerReleased += ToolbarBackground_PointerReleased;
            }

            if (string.IsNullOrEmpty(Title) && Content != null)
                Title = LayoutHelpers.SanitizeTypeName(Content.GetType().Name);

            PointerPressed += ViewContainer_PointerPressed;

            if (Workspace == null)
                Workspace = this.GetVisualParent<Workspace>();
        }


        /* -----------------------------------------------------------------------------------------------------------------------
         * PROPERTIES 
         * ---------------------------------------------------------------------------------------------------------------------*/

        public string Id { get; set; }



        private void ViewContainer_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            Workspace.ActiveWindow = this;
        }

        protected virtual void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            
            Close();
        }



        /* -----------------------------------------------------------------------------------------------------------------------
         * MOVE WINDOW DRAGGING 
         * ---------------------------------------------------------------------------------------------------------------------*/
        Point draggingPointOffset;

        private void ToolbarBackground_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var parentPosition = e.GetPosition(Workspace);
            var pos = new Point(parentPosition.X - draggingPointOffset.X, parentPosition.Y - draggingPointOffset.Y);

            var snappedPosition = Workspace.NearestPoint(pos);

            Bounds = new Rect(snappedPosition, Bounds.Size);
            Position = snappedPosition;
            Size = Bounds.Size;

            Workspace.IsRepositioningChild = false;
            Workspace.RepositionPath.IsVisible = false;
            Workspace.IsHighlightedGridDotsVisible = false;

            PositionChanged?.Invoke(this, Position);
        }

        private void ToolbarBackground_PointerMoved(object sender, PointerEventArgs e)
        {
            if (!Workspace.IsRepositioningChild)
                return;


            var parentPosition = e.GetPosition(Workspace);

            Position = new Point(parentPosition.X - draggingPointOffset.X, parentPosition.Y - draggingPointOffset.Y);

            //Workspace.RepositionBox.Rect = new Rect(Position, Bounds.Size);
            //Workspace.RepositionPath.IsVisible = true;
        }

        private void ToolbarBackground_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            Workspace.IsRepositioningChild = true;
            draggingPointOffset = e.GetPosition(this);
            Workspace.IsHighlightedGridDotsVisible = true;
            Workspace.ActiveWindow = this;
        }



        public Size Size
        {
            get { return GetValue(SizeProperty); }
            set
            {
                SetValue(SizeProperty, value);
                this.Width = value.Width;
                this.Height = value.Height;
            }
        }

        Point? previousValidPosition;
        public Point? PreviousValidPosition
        {
            get => previousValidPosition;
            private set => previousValidPosition = value;
        }

        public Point Position
        {
            get { return GetValue(PositionProperty); }
            set
            {
                SetValue(PositionProperty, value);
                Margin = new Thickness(value.X, value.Y, 0, 0);
                PositionChanging?.Invoke(this, value);
            }
        }


        /* -----------------------------------------------------------------------------------------------------------------------
         * RESIZE DRAGGING 
         * ---------------------------------------------------------------------------------------------------------------------*/

        private void Dragger_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            Workspace.IsReSizingChild = false;
            Workspace.ResizePath.IsVisible = false;
            Workspace.IsHighlightedGridDotsVisible = false;

            var size = Workspace.NearestSize(Workspace.ResizeBox.Bounds.Size);
            Size = size;

            Position = new Point(Workspace.ResizeBox.Rect.X, Workspace.ResizeBox.Rect.Y);
            e.Handled = true;
        }

        private void Dragger_PointerMoved(object sender, Avalonia.Input.PointerEventArgs e)
        {
            if (!Workspace.IsReSizingChild)
                return;

            var pos = e.GetPosition(this);

            Workspace.ResizeBox.Rect = new Rect(Bounds.Position, new Size(pos.X, pos.Y));
            Workspace.ResizePath.IsVisible = true;
            e.Handled = true;
        }

        private void Dragger_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            Workspace.IsReSizingChild = true;
            draggingPointOffset = e.GetPosition(this);
            Workspace.IsHighlightedGridDotsVisible = true;
            e.Handled = true;
        }

        bool invalidPosition;
        public bool InvalidPosition
        {
            get => invalidPosition;
            set
            {
                if (value)
                {
                    BorderBrush = Brushes.Red;
                }
                else if (invalidPosition != value)
                {
                    PreviousValidPosition = Position;
                    BorderBrush = defaultBorderBrush;
                }

                invalidPosition = value;
            }
        }


        public void Hide()
        {
            throw new NotImplementedException();
        }

        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
            throw new NotImplementedException();
        }

        public void BeginMoveDrag(PointerPressedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            var ce = new CancelEventArgs(false);
            Closing?.Invoke(this, ce);

            if(!ce.Cancel)
                Closed?.Invoke(this, EventArgs.Empty);
        }

        public void Close(object dialogResult)
        {
            throw new NotImplementedException();
        }

        public string Title
        {
            get => GetValue(TitleProperty);
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public StartupLocation StartupLocation
        {
            get => GetValue(StartupLocationProperty);
            set
            {
                SetValue(StartupLocationProperty, value);
            }
        }


        public double ToolbarHeight
        {
            get => GetValue(ToolbarHeightProperty);
            set
            {
                SetValue(ToolbarHeightProperty, value);
                toolbarBackground.Height = value;
            }
        }


        public bool OptionsButtonIsVisible
        {
            get => GetValue(OptionsButtonIsVisibleProperty);
            set
            {
                SetValue(OptionsButtonIsVisibleProperty, value);
                optionsButton.IsVisible = value;
            }
        }
        
        
        public List<IControl> ToolbarItems
        {
            get => GetValue(ToolbarItemsProperty);
            set
            {
                foreach (var item in value)
                {
                    if (item is Button button)
                    {
                        button.Classes.Add("ViewContainerToolBarButton");
                        toolbarControls.Children.Add(button);
                    }
                }
                SetValue(ToolbarItemsProperty, value);
            }
        }


        public bool IsResizable
        {
            get => GetValue(IsResizableProperty);
            set
            {
                SetValue(IsResizableProperty, value);
                dragger.IsVisible = value;
            }
        }


        public SizeToContent SizeToContent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool Topmost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public object Owner { get; internal set; }

        public bool CanResize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public WindowStartupLocation WindowStartupLocation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        PixelPoint IWindow.Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Size ClientSize => throw new NotImplementedException();

        public event EventHandler Closed;
        public event EventHandler Selected;
        public event EventHandler<CancelEventArgs> Closing;
        public event EventHandler Activated;
        public event EventHandler Deactivated;
        public event EventHandler<Point> PositionChanged;
        public event EventHandler<Point> PositionChanging;

        public event EventHandler Opened;
    }
}
