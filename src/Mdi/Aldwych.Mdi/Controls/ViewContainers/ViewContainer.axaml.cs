using Aldwych.AvaloniaToolkit.Extensions;
using Aldwych.Mdi.Helpers;
using Avalonia;
using Avalonia.Controls;
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
    public class ViewContainer : Panel, ISelectable, IWindow
    {

        public static readonly StyledProperty<string> TitleProperty = 
            AvaloniaProperty.Register<Workspace, string>(nameof(Title), string.Empty);


        public static readonly StyledProperty<bool> OptionsButtonIsVisibleProperty = 
            AvaloniaProperty.Register<Workspace, bool>(nameof(OptionsButtonIsVisible), false);


        public static readonly StyledProperty<bool> IsResizableProperty = 
            AvaloniaProperty.Register<Workspace, bool>(nameof(IsResizable), false);


        public static readonly StyledProperty<IControl> ContentProperty = 
            AvaloniaProperty.Register<Workspace, IControl>(nameof(Content));


        public static readonly StyledProperty<List<IControl>> ToolbarItemsProperty = 
            AvaloniaProperty.Register<Workspace, List<IControl>>(nameof(ToolbarItems), new List<IControl>());


        public static readonly StyledProperty<double> ToolbarHeightProperty = 
            AvaloniaProperty.Register<Workspace, double>(nameof(ToolbarHeight), 50);


        public static readonly StyledProperty<StartupLocation> StartupLocationProperty = 
            AvaloniaProperty.Register<Workspace, StartupLocation>(nameof(StartupLocation), StartupLocation.CenterParent);


        protected override void OnGotFocus(GotFocusEventArgs e)
        {
            base.OnGotFocus(e);
            contentBorder.IsVisible = true;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            contentBorder.IsVisible = false;
        }


        Workspace parentLayout => this.GetVisualParent<Workspace>();

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
                    Position = new Point(0, parentLayout.Height - this.Height);
                    break;

                case StartupLocation.TopRight:
                    Position = new Point(parentLayout.Width - this.Width, 0);
                    break;

                case StartupLocation.BottomRight:
                    Position = new Point(parentLayout.Width - this.Width, parentLayout.Height - this.Height);
                    break;

                case StartupLocation.CenterParent:
                    Position = new Point(parentLayout.Bounds.Center.X - (this.Bounds.Size.Width / 2), parentLayout.Bounds.Center.Y - (this.Bounds.Size.Height / 2));
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
            var rectangles = parentLayout.Children.OfType<DocumentViewContainer>().Where(x => x != this).Select(x => x.Bounds);
            var root = parentLayout.Bounds;
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


        Border toolbarBackground;
        Border contentBorder;
        Panel viewContent;
        TextBlock title;
        StackPanel toolbarControls;
        Button closeButton;
        Button optionsButton;
        Canvas dragger;
        Border border;

        IBrush defaultBorderBrush;

        public ViewContainer()
        {
            InitializeComponent();

            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
         
            toolbarBackground = this.Get<Border>("PART_TooblarBackground");
            contentBorder = this.Get<Border>("PART_ContentBorder");

            viewContent = this.Get<Panel>("PART_Content");


            title = this.Get<TextBlock>("PART_Title");
            toolbarControls = this.Get<StackPanel>("PART_ToolbarControlsPanel");
            closeButton = this.Get<Button>("PART_CloseButton");
            optionsButton = this.Get<Button>("PART_OptionsButton");
            dragger = this.Get<Canvas>("PART_ResizeDragger");
            border = this.Get<Border>("PART_Border");
            defaultBorderBrush = border.BorderBrush.ToImmutable();

            dragger.PointerPressed += Dragger_PointerPressed;
            dragger.PointerMoved += Dragger_PointerMoved;
            dragger.PointerReleased += Dragger_PointerReleased;


            toolbarBackground.PointerPressed += ToolbarBackground_PointerPressed;
            toolbarBackground.PointerMoved += ToolbarBackground_PointerMoved;
            toolbarBackground.PointerReleased += ToolbarBackground_PointerReleased;
        
            closeButton.Click += CloseButton_Click;

            PointerPressed += ViewContainer_PointerPressed;
        }



        /* -----------------------------------------------------------------------------------------------------------------------
         * PROPERTIES 
         * ---------------------------------------------------------------------------------------------------------------------*/

         public string Id { get; set; }



        private void ViewContainer_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            IsSelected = true;
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
            var parentPosition = e.GetPosition(parentLayout);
            var pos = new Point(parentPosition.X - draggingPointOffset.X, parentPosition.Y - draggingPointOffset.Y);

            var snappedPosition = parentLayout.NearestPoint(pos);

            Bounds = new Rect(snappedPosition, Bounds.Size);
            Position = snappedPosition;
            Size = Bounds.Size;

            parentLayout.IsRepositioningChild = false;
            parentLayout.RepositionPath.IsVisible = false;
            parentLayout.IsHighlightedGridDotsVisible = false;

            PositionChanged?.Invoke(this, Position);
        }

        private void ToolbarBackground_PointerMoved(object sender, PointerEventArgs e)
        {
            if (!parentLayout.IsRepositioningChild)
                return;      


            var parentPosition = e.GetPosition(parentLayout);        

            Position = new Point(parentPosition.X - draggingPointOffset.X, parentPosition.Y - draggingPointOffset.Y);

            parentLayout.RepositionBox.Rect = new Rect(Position, Bounds.Size);
            parentLayout.RepositionPath.IsVisible = true;
        }

        private void ToolbarBackground_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            parentLayout.IsRepositioningChild = true;
            draggingPointOffset = e.GetPosition(this);
            parentLayout.IsHighlightedGridDotsVisible = true;
        }


        public static readonly StyledProperty<Point> PositionProperty = AvaloniaProperty.Register<ViewContainer, Point>(nameof(Position));

        public static readonly StyledProperty<Size> SizeProperty = AvaloniaProperty.Register<ViewContainer, Size>(nameof(Size));

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
            parentLayout.IsReSizingChild = false;
            parentLayout.ResizePath.IsVisible = false;
            parentLayout.IsHighlightedGridDotsVisible = false;

            var size = parentLayout.NearestSize(parentLayout.ResizeBox.Bounds.Size);
            Size = size;

            Position = new Point(parentLayout.ResizeBox.Rect.X, parentLayout.ResizeBox.Rect.Y);
        }

        private void Dragger_PointerMoved(object sender, Avalonia.Input.PointerEventArgs e)
        {
            if (!parentLayout.IsReSizingChild)
                return;

            var pos = e.GetPosition(this);

            parentLayout.ResizeBox.Rect = new Rect(Bounds.Position, new Size(pos.X, pos.Y));
            parentLayout.ResizePath.IsVisible = true;
        }

        private void Dragger_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            parentLayout.IsReSizingChild = true;
            draggingPointOffset = e.GetPosition(this);
            parentLayout.IsHighlightedGridDotsVisible = true;
        }

        bool invalidPosition;
        public bool InvalidPosition
        {
            get => invalidPosition;
            set 
            {
                if (value)
                {
                    border.BorderBrush = Brushes.Red;
                }
                else if (invalidPosition != value)
                {
                    PreviousValidPosition = Position;
                    border.BorderBrush = defaultBorderBrush;
                }

                invalidPosition = value;
            }
        }

        

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
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
                title.Text = value;
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


        public IControl Content
        {
            get => GetValue(ContentProperty);
            set
            {

                if(value is IControl control)
                {
                    SetValue(ContentProperty, control);
                    viewContent.Children.Add(control);
                }              
            }
        }



        public List<IControl> ToolbarItems
        {
            get => GetValue(ToolbarItemsProperty);
            set
            {
                foreach(var item in value)
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


        public bool IsSelected
        {
            get
            {
                if (contentBorder.BorderThickness.Left == 2)
                    return true;
                return false;
            }
            set
            {
                if (value)
                {
                    contentBorder.BorderThickness = new Thickness(2);
                    Selected?.Invoke(this, EventArgs.Empty);
                }
                else
                    contentBorder.BorderThickness = new Thickness(0);
            }
        }

        public SizeToContent SizeToContent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        object IWindow.Content { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public HorizontalAlignment HorizontalContentAlignment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public VerticalAlignment VerticalContentAlignment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsActive => throw new NotImplementedException();

        public bool Topmost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public object Owner => throw new NotImplementedException();

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
