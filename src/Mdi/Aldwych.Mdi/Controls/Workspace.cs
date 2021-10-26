using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.VisualTree;

using System;
using System.Linq;

using Aldwych.Mdi.Helpers;

namespace Aldwych.Mdi.Controls
{
    public enum PointerEventTarget
    {
        EmptySpace,
        Dialog,
        Document,
        View = Dialog | Document,
    }

    public enum StartupLocation
    {
        Manual = 0,
        CenterParent,
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft

    }


    public class Workspace : Canvas
    {
        /* -----------------------------------------------------------------------------------------------------------------------
         * DEPENDANCY PROPERTIES
         * ---------------------------------------------------------------------------------------------------------------------*/
        public static readonly StyledProperty<int> RowCountProperty = AvaloniaProperty.Register<Workspace, int>(nameof(RowCount), 10);
        public static readonly StyledProperty<int> ColumnCountProperty = AvaloniaProperty.Register<Workspace, int>(nameof(ColumnCount), 21);

        public static readonly StyledProperty<IBrush> HighlightDotBrushProperty = AvaloniaProperty.Register<Workspace, IBrush>(nameof(HighlightDotBrush), (IBrush)Brushes.Yellow);
        public static readonly StyledProperty<IBrush> DefaultDotBrushProperty = AvaloniaProperty.Register<Workspace, IBrush>(nameof(DefaultDotBrush), (IBrush)Brushes.White);

        public static readonly StyledProperty<Type> AddNewWindowControlTypeProperty = AvaloniaProperty.Register<Workspace, Type>(nameof(AddNewWindowControlType));


        public static readonly StyledProperty<IWindow> ActiveWindowProperty = AvaloniaProperty.Register<Workspace, IWindow>(nameof(ActiveWindow));


        /* -----------------------------------------------------------------------------------------------------------------------
         * PRIVATE MEMBERS
         * ---------------------------------------------------------------------------------------------------------------------*/

        private bool isDragging;
        private Point? draggingStartPoint;

        private Path selectionPath;
        private RectangleGeometry selectionBox;

        private PointerEventTarget touchType;

        protected double CalculatedColumnWidth { get; set; }
        protected double CalculatedRowHeight { get; set; }


        /* -----------------------------------------------------------------------------------------------------------------------
         * INITIALISATION  
         * ---------------------------------------------------------------------------------------------------------------------*/
        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.ClipToBounds = true;

            _highlightPanel = new Panel() { Name = "HighlightPanel" };
            _highlightPanel.IsVisible = false;
            _highlightPanel.ZIndex = -5;
            Children.Add(_highlightPanel);

            _dotsPanel = new Panel() { ZIndex = -4, IsHitTestVisible = false, Name = "DotsPanel" };
            Children.Add(_dotsPanel);

            _dialogViewFiller = new Panel()
            {
                Background = Brushes.Black,
                Opacity = 0.7f,
                ZIndex = 5,
                IsVisible = false
            };
            Children.Add(_dialogViewFiller);

            selectionBox = new RectangleGeometry();
            selectionPath = new Path() { Name = "SelectionBox" };
            selectionPath.Stroke = Brushes.White;
            selectionPath.StrokeThickness = 1;
            selectionPath.ZIndex = 100;
            selectionPath.IsVisible = false;
            selectionPath.Data = selectionBox;
            Children.Add(selectionPath);

            CreateRePositionControls();
            CreateReSizeControls();


            PointerPressed += HandlePointerPressed;
            PointerReleased += HandlePointerReleased;
            PointerMoved += HandlePointerMoved;

            EffectiveViewportChanged += HandleEffectiveViewportChanged;
            ShowAddNewViewDialogAction = PresentViewPicker;

            ViewsDidChange += (s, e) =>
            {
                var dialogs = Children.OfType<DialogViewContainer>();
                if (dialogs.Any())
                {
                    //we're displaying dialogs! 
                    var topDialog = dialogs.FirstOrDefault();
                    var dialogIndex = topDialog.ZIndex;
                    _dialogViewFiller.ZIndex = dialogIndex - 1;

                    _dialogViewFiller.IsVisible = true;
                }
                else if (_dialogViewFiller.IsVisible)
                    _dialogViewFiller.IsVisible = false;
            };
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ActiveWindowProperty)
            {
                var previousWindow = change.OldValue.Value as MdiWindow;
                var nextWindow = change.NewValue.Value as MdiWindow;

                if (previousWindow != null)
                {
                    previousWindow.IsActive = false;
                    previousWindow.ZIndex = 5;
                }

                if (nextWindow != null)
                {
                    nextWindow.IsActive = true;
                    nextWindow.ZIndex = 10;
                }
            }
        }
        /* -----------------------------------------------------------------------------------------------------------------------
        * PROPERTIES
        * ---------------------------------------------------------------------------------------------------------------------*/

        public Type AddNewWindowControlType
        {
            get => GetValue(AddNewWindowControlTypeProperty);
            set
            {
                if (typeof(IAddWindowProvider).IsAssignableFrom(value) && typeof(IControl).IsAssignableFrom(value))
                    SetValue(AddNewWindowControlTypeProperty, value);
                else
                    throw new Exception("Add Window Provider must implement IAddWindowProvider & IControl");
            }
        }

        public int RowCount
        {
            get { return GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }


        public int ColumnCount
        {
            get { return GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }


        public IBrush HighlightDotBrush
        {
            get { return GetValue(HighlightDotBrushProperty); }
            set { SetValue(HighlightDotBrushProperty, value); }
        }


        public IBrush DefaultDotBrush
        {
            get { return GetValue(DefaultDotBrushProperty); }
            set { SetValue(DefaultDotBrushProperty, value); }
        }


        public bool IsHighlightedGridDotsVisible
        {
            get => _highlightPanel.IsVisible;
            set => _highlightPanel.IsVisible = value;
        }

        public IWindow ActiveWindow
        {
            get { return GetValue(ActiveWindowProperty); }
            set { SetValue(ActiveWindowProperty, value); }
        }


        /* -----------------------------------------------------------------------------------------------------------------------
         * POSITION & SIZE SNAPPING CALCULATION
         * ---------------------------------------------------------------------------------------------------------------------*/

        internal Point NearestPoint(Point input)
        {
            var highResGridWidth = CalculatedColumnWidth / 2;
            var highResGridHeight = CalculatedRowHeight / 2;

            var newX = Math.Round(input.X / highResGridWidth) * highResGridWidth;
            var newY = Math.Round(input.Y / highResGridHeight) * highResGridHeight;

            return new Point(newX, newY);
        }

        internal Size NearestSize(Size input)
        {
            var highResGridWidth = CalculatedColumnWidth / 2;
            var highResGridHeight = CalculatedRowHeight / 2;

            var newX = Math.Round(input.Width / highResGridWidth) * highResGridWidth;
            var newY = Math.Round(input.Height / highResGridHeight) * highResGridHeight;

            return new Size(newX, newY);
        }


        /* -----------------------------------------------------------------------------------------------------------------------
         * CHANGE CHILD VIEW POSITION 
         * ---------------------------------------------------------------------------------------------------------------------*/

        internal Path RepositionPath;
        internal bool IsRepositioningChild;
        internal RectangleGeometry RepositionBox;

        private void CreateRePositionControls()
        {
            RepositionBox = new RectangleGeometry();
            RepositionPath = new Path();
            RepositionPath.Stroke = Brushes.White;
            RepositionPath.StrokeThickness = 1;
            RepositionPath.ZIndex = 100;
            RepositionPath.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
            RepositionPath.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            RepositionPath.IsVisible = false;
            RepositionPath.Data = RepositionBox;
            Children.Add(RepositionPath);
        }


        /* -----------------------------------------------------------------------------------------------------------------------
         * CHANGE CHILD VIEW CHANGE SIZE
         * ---------------------------------------------------------------------------------------------------------------------*/

        internal Path ResizePath;
        internal bool IsReSizingChild;
        internal RectangleGeometry ResizeBox;

        private void CreateReSizeControls()
        {
            ResizeBox = new RectangleGeometry();
            ResizePath = new Path();
            ResizePath.Stroke = Brushes.White;
            ResizePath.StrokeThickness = 1;
            ResizePath.ZIndex = 100;
            ResizePath.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
            ResizePath.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            ResizePath.IsVisible = false;
            ResizePath.Data = ResizeBox;
            Children.Add(ResizePath);
        }


        /* -----------------------------------------------------------------------------------------------------------------------
         * Mouse Events
         * ---------------------------------------------------------------------------------------------------------------------*/

        private void HandlePointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            var visual = this.GetVisualAt(e.GetPosition(this.GetVisualRoot()));
            if (!visual.IsViewContainerChild())
            {
                touchType = PointerEventTarget.EmptySpace;
                _highlightPanel.IsVisible = true;
                draggingStartPoint = e.GetPosition(this);
                isDragging = true;
            }
            else
            {
                touchType = PointerEventTarget.View;
            }
        }

        private void HandlePointerMoved(object sender, Avalonia.Input.PointerEventArgs e)
        {
            if (!isDragging || !draggingStartPoint.HasValue)
                return;

            var position = e.GetPosition(this);

            if (Children.Select(x => x.Bounds.Contains(position)).Any())
                return;

            selectionBox.Rect = new Rect(draggingStartPoint.Value, position);
            selectionPath.IsVisible = true;
        }

        private void HandlePointerReleased(object sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            

            if (touchType == PointerEventTarget.EmptySpace && !IsRepositioningChild && !IsReSizingChild)
                ShowAddNewViewDialogAction();
            else
            {
                _highlightPanel.IsVisible = false;
                selectionPath.IsVisible = false;
                isDragging = false;
            }
        }


        /* -----------------------------------------------------------------------------------------------------------------------
         * VIEW MANAGEMENT   
         * ---------------------------------------------------------------------------------------------------------------------*/

        public void PresentDocumentView(Type type)
        {
            addWindowProviderViewContainer.Close();

            var view = new MdiWindow();
            view.Title = LayoutHelpers.SanitizeTypeName(type.Name);
            view.Content = (IControl)Activator.CreateInstance(type);


            view.Closed += (s, e) =>
            {
                Children.Remove((IControl)s);
                InvalidateVisual();
            };

            view.PositionChanging += (s, e) =>
            {
                view.Opacity = 0.8;


                var windows = Children.OfType<IWindow>().Where(x => x != view);
                var collisionDetected = windows.Any(x => x.Bounds.Intersects(view.Bounds));

                if (collisionDetected)
                    view.InvalidPosition = true;
                else
                    view.InvalidPosition = false;
            };

            view.PositionChanged += (s, e) =>
            {
                if (view.InvalidPosition && view.PreviousValidPosition.HasValue)
                {
                    view.Position = view.PreviousValidPosition.Value;
                    view.InvalidPosition = false;
                }
                view.Opacity = 1.0;
            };

 
            Children.Add(view);
            view.Position = NearestPoint(draggingStartPoint.Value);
            ViewsDidChange?.Invoke(this, new EventArgs());
            view.ZIndex = 11;
        }



        /* -----------------------------------------------------------------------------------------------------------------------
         * VIEW PICKER 
         * ---------------------------------------------------------------------------------------------------------------------*/

        DialogViewContainer addWindowProviderViewContainer;

        public event EventHandler ViewsDidChange;

        private void PresentViewPicker()
        {
            if (addWindowProviderViewContainer == null)
            {
                var addWindowProviderView = (IAddWindowProvider)Activator.CreateInstance(AddNewWindowControlType);
                addWindowProviderView.TargetLayout = this;

                addWindowProviderViewContainer = new DialogViewContainer(addWindowProviderView);
                addWindowProviderViewContainer.Closed += (s, e) =>
                {
                    Children.Remove((IControl)s);
                    addWindowProviderViewContainer = null;
                    ViewsDidChange?.Invoke(this, new EventArgs());
                };

                addWindowProviderViewContainer.ZIndex = 10;
                Children.Add(addWindowProviderViewContainer);
                ViewsDidChange?.Invoke(this, new EventArgs());
            }
        }

        public Action ShowAddNewViewDialogAction { get; set; }



        /* -----------------------------------------------------------------------------------------------------------------------
       * View Size Changed 
       * ---------------------------------------------------------------------------------------------------------------------*/

        private void HandleEffectiveViewportChanged(object sender, Avalonia.Layout.EffectiveViewportChangedEventArgs e)
        {
            DrawBackgroundDots();
            DrawHighlightDots();
            _dialogViewFiller.Width = Bounds.Width;
            _dialogViewFiller.Height = Bounds.Height;
        }



        /* -----------------------------------------------------------------------------------------------------------------------
         * Rendering & Background Dots
         * ---------------------------------------------------------------------------------------------------------------------*/

        public override void Render(DrawingContext context)
        {
            base.Render(context);
        }


        private Panel _dotsPanel;
        private Panel _highlightPanel;
        private Panel _dialogViewFiller;

        Size CalculateGridSize(Size rootSize)
        {
            var rWidth = rootSize.Width;
            var rHeight = rootSize.Height;

            var wRounded = ClosestInteger(rWidth, 100);
            var wBlockCount = wRounded / 100;
            var blockWidth = (rWidth - wBlockCount) / wBlockCount;

            var hRounded = ClosestInteger(rHeight, 105);
            var hBlockCount = hRounded / 105;
            var blockHeight = (rHeight - hBlockCount) / hBlockCount;

            return new Size(blockWidth, blockHeight);

            int ClosestInteger(double value, int factor)
            {
                int nearestMultiple =
                        (int)Math.Round(
                             (value / (double)factor),
                             MidpointRounding.ToZero
                         ) * factor;
                return nearestMultiple;
            }
        }

        protected void DrawBackgroundDots()
        {
            int bmp_width = (int)Bounds.Width;
            int bmp_height = (int)Bounds.Height;

            if ((bmp_width <= 0) || (bmp_height <= 0))
                return;

            var gridBlockSize = CalculateGridSize(Bounds.Size);
            CalculatedColumnWidth = gridBlockSize.Width;
            ColumnCount = (int)Math.Round(Bounds.Width / CalculatedColumnWidth, 0);

            CalculatedRowHeight = gridBlockSize.Height;
            RowCount = (int)Math.Round(Bounds.Height / CalculatedRowHeight, 0);

            LayoutHelpers.GridColumnWidthCoarse = CalculatedColumnWidth;
            LayoutHelpers.GridRowHeightCoarse = CalculatedRowHeight;

            _dotsPanel.Children.Clear();

            for (int colI = 1; colI < (ColumnCount); colI++)
            {
                var yPos = CalculatedColumnWidth * colI;

                for (int rowI = 1; rowI < (RowCount); rowI++)
                {
                    var ellipse = new Ellipse() { Width = 2, Height = 2, Fill = DefaultDotBrush };
                    ellipse.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                    ellipse.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
                    ellipse.Opacity = 0.5;
                    var xPos = CalculatedRowHeight * rowI;

                    var mx = xPos - (ellipse.Width / 2);
                    var my = yPos - (ellipse.Height / 2);

                    ellipse.Margin = new Thickness(my, mx, 0, 0);
                    _dotsPanel.Children.Add(ellipse);
                }
            }

        }

        protected void DrawHighlightDots()
        {
            int bmp_width = (int)Bounds.Width;
            int bmp_height = (int)Bounds.Height;

            if ((bmp_width <= 0) || (bmp_height <= 0))
            {
                return;
            }

            var cc = ColumnCount * 2;
            var rc = RowCount * 2;

            var gridColumnWidth = CalculatedColumnWidth / 2;
            var gridRowHeight = CalculatedRowHeight / 2;

            LayoutHelpers.GridColumnWidthFine = gridColumnWidth;
            LayoutHelpers.GridRowHeightFine = gridRowHeight;

            _highlightPanel.Children.Clear();

            //Add small dots
            for (int colI = 1; colI < (cc); colI++)
            {
                var yPos = gridColumnWidth * colI;

                for (int rowI = 1; rowI < (rc); rowI++)
                {
                    var highlightEllipse = new Ellipse() { Width = 2, Height = 2, Fill = HighlightDotBrush };
                    highlightEllipse.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                    highlightEllipse.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
                    highlightEllipse.Opacity = 0.3;
                    var xPos = gridRowHeight * rowI;

                    highlightEllipse.Margin = new Thickness(yPos, xPos, 0, 0);
                    _highlightPanel.Children.Add(highlightEllipse);
                }
            }

            //Add larger dots
            for (int colI = 1; colI < (ColumnCount); colI++)
            {
                var yPos = CalculatedColumnWidth * colI;

                for (int rowI = 1; rowI < (RowCount); rowI++)
                {
                    var ellipse = new Ellipse() { Width = 4, Height = 4, Fill = HighlightDotBrush };
                    ellipse.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                    ellipse.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
                    ellipse.Opacity = 0.5;
                    var xPos = CalculatedRowHeight * rowI;

                    var mx = xPos - (ellipse.Width / 2);
                    var my = yPos - (ellipse.Height / 2);


                    ellipse.Margin = new Thickness(my, mx, 0, 0);
                    _highlightPanel.Children.Add(ellipse);
                }
            }

        }
    }
}
