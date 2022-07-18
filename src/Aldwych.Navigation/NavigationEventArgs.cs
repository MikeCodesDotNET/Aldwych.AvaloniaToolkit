using System;

namespace Aldwych.Navigation
{
    internal delegate void NavigatedEventHandler(object sender, NavigationEventArgs e);
    internal delegate void NavigationStoppedEventHandler(object sender, NavigationEventArgs e);

    internal class NavigationEventArgs
    {
        internal NavigationEventArgs(object content, NavigationMode mode,
            NavigationTransitionInfo navInfo, object param,
            Type srcPgType)
        {
            Content = content;
            NavigationMode = mode;
            NavigationTransitionInfo = navInfo;
            Parameter = param;
            SourcePageType = srcPgType;
        }

        //public Uri Uri { get; set; }
        public object Content { get; }
        public NavigationMode NavigationMode { get; }
        public object Parameter { get; }
        public Type SourcePageType { get; }
        public NavigationTransitionInfo NavigationTransitionInfo { get; }
    }

    internal class FrameNavigationOptions
    {
        public NavigationTransitionInfo TransitionInfoOverride { get; set; }
        public bool IsNavigationStackEnabled { get; set; }
    }

    internal delegate void NavigatingCancelEventHandler(object sender, NavigatingCancelEventArgs e);

    internal class NavigatingCancelEventArgs
    {
        internal NavigatingCancelEventArgs(NavigationMode mode, NavigationTransitionInfo info,
            object param, Type srcType)
        {
            NavigationMode = mode;
            NavigationTransitionInfo = info;
            Parameter = param;
            SourcePageType = srcType;
        }

        public bool Cancel { get; set; }
        public NavigationMode NavigationMode { get; }
        public Type SourcePageType { get; }
        public NavigationTransitionInfo NavigationTransitionInfo { get; }
        public object Parameter { get; }
    }

    internal delegate void NavigationFailedEventHandler(object sender, NavigationFailedEventArgs e);

    internal class NavigationFailedEventArgs
    {
        internal NavigationFailedEventArgs(Exception ex, Type srcPageType)
        {
            Exception = ex;
            SourcePageType = srcPageType;
        }

        public bool Handled { get; set; }
        public Exception Exception { get; }
        public Type SourcePageType { get; }
    }

    internal enum NavigationMode
    {
        New = 0,
        Back,
        Forward,
        Refresh
    }
}
