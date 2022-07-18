using Aldwych.AvaloniaToolkit.ViewModels;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;

using System;
using System.Linq;

namespace Aldwych.Navigation
{  
    public class Frame : ContentControl
    {
        public Frame()
        {            
        }

        public static readonly DirectProperty<Frame, Type> SourcePageTypeProperty =
            AvaloniaProperty.RegisterDirect<Frame, Type>(nameof(SourcePageType),
                x => x.SourcePageType, (x, v) => x.SourcePageType = v);
               

        public static readonly DirectProperty<Frame, Type> CurrentSourcePageTypeProperty =
            AvaloniaProperty.RegisterDirect<Frame, Type>(nameof(CurrentSourcePageType),
                x => x.CurrentSourcePageType);


        public Type SourcePageType
        {
            get => _sourcePageType;
            set => SetAndRaise(SourcePageTypeProperty, ref _sourcePageType, value);
        }
            
      
        public Type CurrentSourcePageType => Content?.GetType();

       
        internal PageStackEntry CurrentEntry { get; set; }

        internal event NavigatedEventHandler Navigated;
        internal event NavigatingCancelEventHandler Navigating;
        internal event NavigationFailedEventHandler NavigationFailed;
        internal event NavigationStoppedEventHandler NavigationStopped;


        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == ContentProperty)
            {
                if (change.NewValue.GetValueOrDefault() == null)
                {
                    CurrentEntry = null;
                }
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _presenter = e.NameScope.Find<ContentPresenter>("ContentPresenter");
        }

        public bool Navigate<T>()
        {
            return NavigateCore(new PageStackEntry(typeof(T), null, null), NavigationMode.New);
        }

        public bool Navigate<T>(NavigationTransitionInfo infoOverride)
        {
            return NavigateCore(new PageStackEntry(typeof(T), null, infoOverride), NavigationMode.New);
        }

        public bool Navigate(Type sourcePageType, object parameter, NavigationTransitionInfo infoOverride)
        {
            return NavigateCore(new PageStackEntry(sourcePageType, parameter, infoOverride), NavigationMode.New);
        }

   
        public bool Navigate(Type sourcePageType) => Navigate(sourcePageType, null, null);

        private bool NavigateCore(PageStackEntry entry, NavigationMode mode)
        {
            try
            {
                _isNavigating = true;

                var ea = new NavigatingCancelEventArgs(mode,
                    entry.NavigationTransitionInfo,
                    entry.Parameter,
                    entry.SourcePageType);

                Navigating?.Invoke(this, ea);

                if (ea.Cancel)
                {
                    OnNavigationStopped(entry, mode);
                    return false;
                }

                //Navigate to new page
                var prevEntry = CurrentEntry;
                CurrentEntry = entry;

                if (CurrentEntry.Instance == null)
                {
                    var page = CreatePageAndCacheIfNecessary(entry.SourcePageType);
                    if (page == null)
                        return false;

                    CurrentEntry.Instance = page;
                }

                SetContentAndAnimate(entry);
                       
                var navEA = new NavigationEventArgs(
                    CurrentEntry.Instance,
                    mode, entry.NavigationTransitionInfo,
                    entry.Parameter,
                    entry.SourcePageType);

                SourcePageType = entry.SourcePageType;

                Navigated?.Invoke(this, navEA);
                return true;

            }
            catch (Exception ex)
            {
                NavigationFailed?.Invoke(this, new NavigationFailedEventArgs(ex, entry.SourcePageType));

                //I don't really want to throw an exception and break things. Just return false
                return false;
            }
            finally
            {
                _isNavigating = false;
            }
        }

        private void OnNavigationStopped(PageStackEntry entry, NavigationMode mode)
        {
            NavigationStopped?.Invoke(this, new NavigationEventArgs(entry.Instance,
                mode, entry.NavigationTransitionInfo, entry.Parameter, entry.SourcePageType));
        }


        ViewModelResolver vl = (ViewModelResolver)Application.Current.DataTemplates.FirstOrDefault(v => v.GetType() == typeof(ViewModelResolver));


        private IControl CreatePageAndCacheIfNecessary(Type srcPageType)
        {
            var vm = Activator.CreateInstance(srcPageType);
            if (vl.Match(vm))
                return vl.Build(vm) as IControl;
            else
                return Activator.CreateInstance(srcPageType) as IControl;
        }

    
        private void SetContentAndAnimate(PageStackEntry entry)
        {
            if (entry == null)
                return;

            Content = entry.Instance;

            if (_presenter != null)
            {
                //Default to entrance transition
                entry.NavigationTransitionInfo = entry.NavigationTransitionInfo ?? new EntranceNavigationTransitionInfo();

                // Very busy pages will delay loading b/c layout & render has to occur first
                // Posting this helps a little bit, but not much
                // Not really sure how to get the transition to occur while the page is loading
                // so speed is comparable to WinUI...this may be an Avalonia limitation???
                Dispatcher.UIThread.Post(() =>
                {
                    entry.NavigationTransitionInfo.RunAnimation(_presenter);
                }, DispatcherPriority.Loaded);
            }
        }

        private ContentPresenter _presenter;
        private bool _isNavigating = false;
        private Type _sourcePageType;
    }
}


