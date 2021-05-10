using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCountdowns.Core.Services.Navigation
{
    public interface INavigationService
    {
        void Navigate<TViewModel>();

        void GoBack();

        bool CanGoBack { get; }
    }
}
