using System.IO;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Aldwych.AvaloniaToolkit.Controls.IO
{
    public enum DoesNotExistAction
    {
        Ignore,
        Error,
        Create
    }

    public abstract class PathTextBoxBase : TemplatedControl
    {
        public static readonly StyledProperty<string> ButtonTextProperty = AvaloniaProperty.Register<PathTextBoxBase, string>(nameof(ButtonText), "...");

        public static readonly StyledProperty<string> DialogTitleProperty = AvaloniaProperty.Register<PathTextBoxBase, string>(nameof(DialogTitle));

        public static readonly StyledProperty<string> PathProperty = AvaloniaProperty.Register<PathTextBoxBase, string>(nameof(Path), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        public static readonly StyledProperty<string> WatermarkProperty = AvaloniaProperty.Register<PathTextBoxBase, string>(nameof(Watermark));

        public static readonly StyledProperty<bool> IsValidProperty = AvaloniaProperty.Register<PathTextBoxBase, bool>(nameof(IsValid), true);

        public static readonly StyledProperty<DoesNotExistAction> DoesNotExistActionProperty = AvaloniaProperty.Register<PathTextBoxBase, DoesNotExistAction>(nameof(DoesNotExistAction));

        public static readonly StyledProperty<FileSystemInfo> FileSystemInfoProperty = AvaloniaProperty.Register<PathTextBoxBase, FileSystemInfo>(nameof(FileSystemInfo));

        public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<PathTextBoxBase, bool>(nameof(IsReadOnly));


        public static readonly StyledProperty<string>HeaderProperty = AvaloniaProperty.Register<PathTextBoxBase, string>(nameof(Header));

        public string Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }


        public DoesNotExistAction DoesNotExistAction
        {
            get { return GetValue(DoesNotExistActionProperty); }
            set { SetValue(DoesNotExistActionProperty, value); }
        }

        public bool IsValid
        {
            get { return GetValue(IsValidProperty); }
            protected set { SetValue(IsValidProperty, value); }
        }

        public string Watermark
        {
            get { return GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }


        public string ButtonText
        {
            get { return GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }


        public string DialogTitle
        {
            get { return GetValue(DialogTitleProperty); }
            set { SetValue(DialogTitleProperty, value); }
        }


        protected abstract void ValidatePath(string path);


        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _browseButton = e.NameScope.Find<Button>("PART_BrowseButton");
            _browseButton.Click += BrowseButton_Click;

            _pathTextBox = e.NameScope.Find<TextBox>("PART_PathTextBox");
            _pathTextBox.LostFocus += _pathTextBox_LostFocus;

        }

        private void _pathTextBox_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ValidatePath(_pathTextBox.Text);
        }


        protected abstract void BrowseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e);


        public FileSystemInfo FileSystemInfo
        {
            get { return GetValue(FileSystemInfoProperty); }
            set { SetValue(FileSystemInfoProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public string Path
        {
            get { return GetValue(PathProperty); }
            set
            {
                SetValue(PathProperty, value);
                ValidatePath(value);
            }
        }

        private Button _browseButton { get; set; }
        private TextBox _pathTextBox { get; set; }

    }
}
