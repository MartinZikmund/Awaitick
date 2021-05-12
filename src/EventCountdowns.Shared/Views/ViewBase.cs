using EventCountdowns.Core.Infrastructure;
using EventCountdowns.Core.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace EventCountdowns.Views
{
    public partial class ViewBase<TViewModel> : Page
        where TViewModel : ViewModel
    {
        private TViewModel? _model = null;

        public ViewBase()
        {
            //TODO: Move to later?
            DataContext = Model;
        }

        public virtual TViewModel Model => _model ??= IoC.GetRequiredService<TViewModel>();

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await Model.LoadAsync(e.Parameter);
        }
    }
}
