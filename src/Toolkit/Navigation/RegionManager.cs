using Aldwych.AvaloniaToolkit.Controls;
using Aldwych.AvaloniaToolkit.ViewModels;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aldwych.AvaloniaToolkit.Navigation
{
    public enum TransitionType
    {
        None,
        Slide,
        Crossfade,
    }

    internal class RegionManager
    {

        private static readonly Lazy<RegionManager> lazy = new Lazy<RegionManager>(() => new RegionManager());

        public static RegionManager Instance { get { return lazy.Value; } }

        private RegionManager()
        {            
        }


        /// <summary>
        /// The name of the region maps to a list of controls that have their region names attached property set as the key. 
        /// </summary>
        private Dictionary<string, List<IContentControl>> regionContent = new Dictionary<string, List<IContentControl>>();


        public void RegisterRegion(string regionName, IContentControl hostControl)
        {
            var hosts = regionContent.GetOrCreate(regionName);
            if (hosts.Contains(hostControl))
                return; //Already added 

            hosts.Add(hostControl);
        }

        public void PushViewModel(Type viewModelType, string regionName, TransitionType transition = TransitionType.None)
        {
            if (string.IsNullOrEmpty(regionName)) throw new ArgumentNullException(nameof(regionName));
            var vm = CreateViewModel(viewModelType);
            SetRegionHosts(regionName, vm, transition);
        }

        public void PushViewModel<T>(string regionName, TransitionType transition = TransitionType.None) where T : ViewModelBase
        {
            List<IContentControl> hosts;
            regionContent.TryGetValue(regionName, out hosts);
            if (hosts != null && hosts.Any())
            {
                foreach (var host in hosts)
                {
                    if (host.Content is Frame frame)
                    {
                        frame.Navigate<T>();
                    }
                    else
                    {
                        frame = new Frame();
                        host.Content = frame;
                        frame.Navigate<T>();
                    }
                }
            }
        }


        public void PushViewModel<T>(Action<T> setViewModel, string regionName, TransitionType transition = TransitionType.None) where T : ViewModelBase
        {
            if (string.IsNullOrEmpty(regionName)) throw new ArgumentNullException(nameof(regionName));

            T vm = CreateViewModel(typeof(T)) as T;
            setViewModel?.Invoke(vm);
            SetRegionHosts(regionName, vm, transition);
        }



        private void SetRegionHosts(string regionName, object content, TransitionType transition = TransitionType.None)
        {
            if (string.IsNullOrEmpty(regionName)) throw new ArgumentNullException(nameof(regionName));
            if(content == null) throw new ArgumentNullException(nameof(content));

            List<IContentControl> hosts;
            regionContent.TryGetValue(regionName, out hosts);
            if (hosts != null && hosts.Any())
            {
                foreach (var host in hosts)
                {                    
                    if (host.Content is Frame frame)
                    {
                        frame.Navigate(content);
                    }
                    else
                    {
                        frame = new Frame() { Background = Brushes.Orange };
                        host.Content = frame;
                        frame.Navigate(content);
                    }
                }
            }
        }

        private object CreateViewModel(Type type)
        {
            var vm = Activator.CreateInstance(type);
            if (vm == null) throw new ArgumentNullException("ViewModel");
            return vm;
        }
    }
}
