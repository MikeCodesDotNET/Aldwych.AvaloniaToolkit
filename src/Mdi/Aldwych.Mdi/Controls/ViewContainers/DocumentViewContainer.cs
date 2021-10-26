using Avalonia.Controls;
using System;
using Aldwych.Mdi.Helpers;

namespace Aldwych.Mdi.Controls
{
    public class DocumentViewContainer : ViewContainer
    {
        public DocumentViewContainer() { }

        public DocumentViewContainer(Type contentType)
        {
            Title = LayoutHelpers.SanitizeTypeName(contentType.Name);
            Content = (IControl)Activator.CreateInstance(contentType);
        }     
    }
}
