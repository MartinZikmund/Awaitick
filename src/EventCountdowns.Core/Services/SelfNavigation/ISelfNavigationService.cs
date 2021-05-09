using MvvmCross.Core.ViewModels;

namespace EventCountdowns.Core.Services.SelfNavigation
{
    public interface ISelfNavigationService
    {
        void ShowViewModel<T>( object data ) where T : MvxViewModel;
    }
}