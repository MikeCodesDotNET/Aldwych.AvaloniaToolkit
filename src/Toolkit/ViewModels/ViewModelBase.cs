using Aldwych.AvaloniaToolkit.Navigation;
using ReactiveUI;
using System;

namespace Aldwych.AvaloniaToolkit.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        protected void PushViewModel<T>(string regionName, TransitionType transition = TransitionType.None) where T : ViewModelBase
        {
            RegionManager.Instance.PushViewModel<T>(regionName, transition);
        }

        protected void PushViewModel<T>(Action<T> setViewModel, string regionName, TransitionType transition = TransitionType.None) where T : ViewModelBase
        {
            RegionManager.Instance.PushViewModel<T>(setViewModel, regionName, transition);
        }

        protected void PushViewModel(Type viewModelType, string regionName, TransitionType transition = TransitionType.None)
        {
            RegionManager.Instance.PushViewModel(viewModelType, regionName, transition);
        }
    }
}
