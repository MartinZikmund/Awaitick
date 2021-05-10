using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace EventCountdowns.Core.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private const string ModelSuffix = "Model";
        private Type[] _views;

        public NavigationService()
        {
            FindViews();
        }

        public bool CanGoBack => ShellView.Instance.AppFrame.CanGoBack;

        public void GoBack()
        {
            if (ShellView.Instance.AppFrame.CanGoBack)
            {
                ShellView.Instance.AppFrame.GoBack();
            }
        }

        public void Navigate<TViewModel>()
        {
            var view = FindViewForViewModel<TViewModel>();
            if (view == null)
            {
                throw new InvalidOperationException($"View for {typeof(TViewModel).Name} view model does not exist.");
            }
            ShellView.Instance.AppFrame.Navigate(view);
        }

        private void FindViews()
        {
            var viewBase = typeof(ViewBase);
            _views = viewBase.Assembly.GetTypes().Where(v => !v.IsAbstract && v.IsSubclassOf(viewBase)).ToArray();
        }

        private Type FindViewForViewModel<TViewModel>()
        {
            var name = typeof(TViewModel).Name;
            var viewName = name.Substring(0, name.Length - ModelSuffix.Length);
            return _views.FirstOrDefault(v => v.Name == viewName);
        }
    }
}
