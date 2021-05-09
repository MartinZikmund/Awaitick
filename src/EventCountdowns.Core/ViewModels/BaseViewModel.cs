using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using EventCountdowns.Core.Services.InAppPurchases;

namespace EventCountdowns.Core.ViewModels
{
    public class BaseViewModel : MvxViewModel
    {
        public bool UserPremium => Mvx.Resolve<IInAppPurchaseService>().HasUserAnyProduct();

        private bool _isWorking;

        public bool IsWorking
        {
            get
            {
                return _isWorking;
            }
            set { SetProperty( ref _isWorking, value ); }
        }
    }
}
