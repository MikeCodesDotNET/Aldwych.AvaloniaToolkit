using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aldwych.AvaloniaToolkit.ViewModels
{
    public class ViewModelResolver
    {
        public Control ResolveView<TVM>() where TVM : ViewModelBase
        {
            var type = typeof(TVM);
            return ResolveView(type);
        }

        public Control ResolveView(Type viewModelType)
        {
            if (viewModelToViewMap.ContainsKey(viewModelType))
            {
                var viewType = viewModelToViewMap[viewModelType];
                return (Control)Activator.CreateInstance(viewType);
            }

            if (discoveredViewModels.Contains(viewModelType))
            {
                var name = viewModelType.Name;
                var viewName = name.Replace("ViewModel", "View");

                if (discoveredViews.ContainsKey(viewName))
                {
                    var viewType = discoveredViews[viewName];
                    return (Control)Activator.CreateInstance(viewType);
                }

                viewName = name.Replace("ViewModel", "");
                if (discoveredWindows.ContainsKey(viewName))
                {
                    var viewType = discoveredViews[viewName];
                    return (Control)Activator.CreateInstance(viewType);
                }
            }

            throw new KeyNotFoundException();
        }


        public void Map<TView, TViewModel>()
        {
            viewModelToViewMap.Add(typeof(TViewModel), typeof(TView));
            viewToViewModelMap.Add(typeof(TView), typeof(TViewModel));
        }

        public void Map(Type view, Type viewModel)
        {
            viewModelToViewMap.Add(viewModel, view);
            viewToViewModelMap.Add(view, viewModel);
        }


        public ViewModelResolver(Assembly[] searhAssemblies)
        {
            var vmType = typeof(ViewModelBase);
            discoveredViewModels = searhAssemblies.SelectMany(s => s.GetTypes()).Where(p => vmType.IsAssignableFrom(p)).ToList();


            var views = searhAssemblies.SelectMany(s => s.GetTypes()).Where(p => p.Name.Contains("View"));
            foreach (var vm in views)
            {
                if (vm.Name.Contains("ViewModel"))
                    continue;

                discoveredViews.TryAdd(vm.Name, vm);
            }

            var windows = searhAssemblies.SelectMany(s => s.GetTypes()).Where(p => p.Name.Contains("Window"));
            foreach (var vm in windows)
                discoveredWindows.TryAdd(vm.Name, vm);
        }



        private Dictionary<Type, Type> viewToViewModelMap = new Dictionary<Type, Type>();
        private Dictionary<Type, Type> viewModelToViewMap = new Dictionary<Type, Type>();

        private List<Type> discoveredViewModels = new List<Type>();

        private Dictionary<string, Type> discoveredViews = new Dictionary<string, Type>();
        private Dictionary<string, Type> discoveredWindows = new Dictionary<string, Type>();


    }
}
