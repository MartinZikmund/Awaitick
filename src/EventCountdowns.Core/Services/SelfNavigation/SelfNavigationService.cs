using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;

namespace EventCountdowns.Core.Services.SelfNavigation
{
    public class SelfNavigationService : MvxNavigatingObject, ISelfNavigationService
    {
        public void ShowViewModel<T>( object data ) where T : MvxViewModel
        {
            base.ShowViewModel<T>( data );
        }
    }
}
