using Aldwych.Mdi.Controls;
using Avalonia.Controls;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldwych.Mdi.Helpers
{
    internal static class ViewExtensions
    {
        public static bool IsViewContainerChild(this IVisual visual)
        {
            if (visual.VisualParent.GetType().IsAssignableFrom(typeof(DocumentViewContainer)) ||
                visual.VisualParent.GetType().IsAssignableFrom(typeof(DialogViewContainer)) ||
                visual.VisualParent.GetType().IsAssignableFrom(typeof(MdiWindow)) ||
                visual.VisualParent.GetType().IsAssignableFrom(typeof(ViewContainer)) ||
                visual.GetType().IsAssignableFrom(typeof(MdiWindow))) 
                {
                    return true;
                }

            if (visual.GetType() == typeof(Window))
                return false;

            if (visual.GetType() == typeof(Workspace))
                return false;


            return visual.VisualParent.IsViewContainerChild();
        }
    }
}
