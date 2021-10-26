using System;
using Aldwych.Mdi.Helpers;
using Avalonia.Controls;

namespace Aldwych.Mdi.Controls
{
    public class DialogViewContainer : ViewContainer
    {
        public DialogViewContainer(Type contentType)
        {
            Title = LayoutHelpers.SanitizeTypeName(contentType.Name);
            Content = (IControl)Activator.CreateInstance(contentType);
            SetDefaults();
        }

        public DialogViewContainer(object obj)
        {
            if (obj != null && obj is IControl)
            {
                Title = LayoutHelpers.SanitizeTypeName(obj.GetType().Name);
                Content = (IControl)obj;
                SetDefaults();
            }
        }

        public DialogViewContainer() { }

        void SetDefaults()
        {
            IsResizable = false;
            OptionsButtonIsVisible = false;
            Width = LayoutHelpers.GridColumnWidthCoarse * 8;
            Height = LayoutHelpers.GridRowHeightCoarse * 4;
        }
    }
}
