#nullable enable

using System;
using System.Windows.Input;
using EventCountdowns.Core.Services;
using EventCountdowns.Core.Services.Dialogs;
using EventCountdowns.Core.Services.InAppPurchases;

namespace EventCountdowns.Core.ViewModels
{
    public class BuyMeCoffeeViewModel : BaseViewModel
    {
        private readonly IInAppPurchaseService _inAppPurchaseService;
        private readonly IDialogService _messageDialogService;
        private readonly ILocalizationService _localizationService;

        public BuyMeCoffeeViewModel(
            IInAppPurchaseService inAppPurchaseService, 
            IDialogService messageDialogService, 
            ILocalizationService localizationService)
        {
            _inAppPurchaseService = inAppPurchaseService ?? throw new ArgumentNullException(nameof(inAppPurchaseService));
            _messageDialogService = messageDialogService ?? throw new ArgumentNullException(nameof(messageDialogService));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        }

        private ICommand _donateCommand = null;

        public ICommand DonateCommand => _donateCommand ?? (_donateCommand = new DelegateCommand<string>(Donate));

        private async void Donate(string coffeeSize)
        {
            InAppProducts product = InAppProducts.LargeCoffee;
            switch (coffeeSize.ToLowerInvariant())
            {
                case "small":
                    {
                        product = InAppProducts.SmallCoffee;
                        break;
                    }
                case "medium":
                    {
                        product = InAppProducts.MediumCoffee;
                        break;
                    }
                case "large":
                    {
                        product = InAppProducts.LargeCoffee;
                        break;
                    }
                case "giga":
                    {
                        product = InAppProducts.GigaCoffee;
                        break;
                    }
            }
            IsWorking = true;
            var result = await _inAppPurchaseService.PurchaseAsync(product);
            if (result)
            {
                //show dialog
                await _messageDialogService.ShowAsync(_localizationService.AppName, _localizationService.CoffeeThankYou);
            }
            IsWorking = false;
        }
    }
}
