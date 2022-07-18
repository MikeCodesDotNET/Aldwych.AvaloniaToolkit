using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldwych.Navigation
{
    internal class PageStackEntry
    {
        public PageStackEntry(Type sourcePageType, object parameter, NavigationTransitionInfo navigationTransitionInfo)
        {
            NavigationTransitionInfo = navigationTransitionInfo;
            SourcePageType = sourcePageType;
            Parameter = parameter;
        }

        public Type SourcePageType { get; set; }
        public NavigationTransitionInfo NavigationTransitionInfo { get; internal set; }
        public object Parameter { get; set; }
        internal IControl Instance { get; set; }
    }
}
