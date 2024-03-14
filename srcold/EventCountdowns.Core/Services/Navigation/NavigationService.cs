using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Windows.UI.Xaml.Controls;

namespace EventCountdowns.Core.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private const string ModelSuffix = "Model";

        private readonly Dictionary<Type, Type> _viewModelToPageMap = new();
        private readonly IFrameAccessor _frameAccessor;

        public NavigationService(IFrameAccessor frameAccessor)
        {
            _frameAccessor = frameAccessor ?? throw new ArgumentNullException(nameof(frameAccessor));
        }

        public bool CanGoBack => _frameAccessor.GetFrame().CanGoBack;

        public void GoBack()
        {
            var frame = _frameAccessor.GetFrame();
            if (frame.CanGoBack)
            {
                frame.GoBack();
            }
        }

        public void Navigate<TViewModel>()
        {
            var view = FindViewForViewModel<TViewModel>();

            _frameAccessor.GetFrame().Navigate(view);
        }

        private Type FindViewForViewModel<TViewModel>()
        {
            if (!_viewModelToPageMap.TryGetValue(typeof(TViewModel), out var pageType))
            {
                throw new InvalidOperationException($"ViewModel type {typeof(TViewModel).Name} is not registered for navigation.");
            }

            return pageType;
        }

        public void Navigate<TViewModel>(object navigationModel)
        {
            var view = FindViewForViewModel<TViewModel>();
            _frameAccessor.GetFrame().Navigate(view, navigationModel);
        }

        public INavigationService RegisterForNavigation<TViewModel, TPage>()
            where TPage : Page
        {
            _viewModelToPageMap[typeof(TViewModel)] = typeof(TPage);
            return this;
        }
    }
}
