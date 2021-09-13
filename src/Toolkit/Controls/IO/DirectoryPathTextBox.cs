using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Aldwych.AvaloniaToolkit.Controls
{
    using Aldwych.AvaloniaToolkit.Controls.IO;

    public class DirectoryPathTextBox : PathTextBoxBase
    {
        protected override async void BrowseButton_Click(object? sender, RoutedEventArgs e)
        {
            var root = this.VisualRoot as Window;
            if (root == null)
            {
                throw new Exception("Couldn't find visual root");
            }

            var dialog = new OpenFolderDialog
            {
                Directory = Path,
                Title = DialogTitle
            };
            var result = await dialog.ShowAsync(root);
            Path = result;
        }

        protected override void ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;


            if (Directory.Exists(path))
            {
                IsValid = true;
            }
            else
            {
                if (DoesNotExistAction == DoesNotExistAction.Create)
                {
                    var dir = Directory.CreateDirectory(path);
                    IsValid = dir.Exists;
                }
                if (DoesNotExistAction == DoesNotExistAction.Error)
                {
                    IsValid = false;
                }
            }

        }
    }
}
