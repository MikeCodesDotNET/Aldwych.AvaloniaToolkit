using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;


namespace Aldwych.AvaloniaToolkit.Controls
{
    using Aldwych.AvaloniaToolkit.Controls.IO;

    public class FilePathTextBox : PathTextBoxBase
    {
        public static readonly StyledProperty<List<FileDialogFilter>> FileFiltersProperty = AvaloniaProperty.Register<PathTextBoxBase, List<FileDialogFilter>>(nameof(FileFilters), new List<FileDialogFilter>());

        public List<FileDialogFilter> FileFilters
        {
            get { return GetValue(FileFiltersProperty); }
            set { SetValue(FileFiltersProperty, value); }
        }

        protected override async void BrowseButton_Click(object? sender, RoutedEventArgs e)
        {
            var root = this.VisualRoot as Window;
            if (root == null)
            {
                throw new Exception("Couldn't find visual root");
            }

            var dialog = new OpenFileDialog();
            dialog.AllowMultiple = false;
            dialog.Filters = FileFilters;
            var result = await dialog.ShowAsync(root);
            Path = result.FirstOrDefault();
        }

        protected override void ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (File.Exists(path))
            {
                IsValid = true;
            }
            else
            {
                if (DoesNotExistAction == DoesNotExistAction.Create)
                {
                    File.Create(path);
                    IsValid = true;
                }
                if (DoesNotExistAction == DoesNotExistAction.Error)
                {
                    IsValid = false;
                }
            }

        }
    }
}
