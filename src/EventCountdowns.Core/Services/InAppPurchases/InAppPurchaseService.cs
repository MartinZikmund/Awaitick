using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.UI.Popups;
using EventCountdowns.Core.Services.InAppPurchases;
using EventCountdowns.Core.Infrastructure;

namespace EventCountdowns.Core.Services
{
    public class InAppPurchaseService : IInAppPurchaseService
    {
        private LicenseInformation _licenseInformation = null;

        private void InitializeLicenseInformation()
        {
            if (_licenseInformation == null)
            {
#if DEBUG
                _licenseInformation = CurrentAppSimulator.LicenseInformation;
#else
                _licenseInformation = CurrentApp.LicenseInformation;
#endif
            }
        }

        public bool HasUserAnyProduct()
        {
            try
            {
                InitializeLicenseInformation();
                string[] durableIds = new[]
                {
                    GetProductId(InAppProducts.SmallCoffee), GetProductId(InAppProducts.MediumCoffee),
                    GetProductId(InAppProducts.LargeCoffee), GetProductId(InAppProducts.GigaCoffee)
                };
                return (from d in durableIds where _licenseInformation.ProductLicenses[d].IsActive select d).Any();
            }
            catch (Exception ex)
            {
                //TODO:Track exception
                return false;
            }
        }

        private string GetProductId(InAppProducts product, bool durable = true)
        {
            if (durable)
            {
                switch (product)
                {
                    case InAppProducts.SmallCoffee:
                        return "EventCountdownsSmallCoffee";
                    case InAppProducts.MediumCoffee:
                        return "EventCountdownsMediumCoffee";
                    case InAppProducts.LargeCoffee:
                        return "EventCountdownsLargeCoffee";
                    case InAppProducts.GigaCoffee:
                        return "EventCountdownsGigaCoffee";
                    default:
                        throw new ArgumentOutOfRangeException(nameof(product), product, null);
                }
            }
            else
            {
                switch (product)
                {
                    case InAppProducts.SmallCoffee:
                        return "EventCountdownsDemiCoffee";
                    case InAppProducts.MediumCoffee:
                        return "EventCountdownsGrandeCoffee";
                    case InAppProducts.LargeCoffee:
                        return "EventCountdownsVentiCoffee";
                    case InAppProducts.GigaCoffee:
                        return "EventCountdownsTrentaCoffee";
                    default:
                        throw new ArgumentOutOfRangeException(nameof(product), product, null);
                }
            }

        }

        public async Task<bool> PurchaseAsync(InAppProducts product)
        {
            try
            {
                InitializeLicenseInformation();
                var productId = GetProductId(product);

                if (!_licenseInformation.ProductLicenses[productId].IsActive)
                {
#if DEBUG
                    await CurrentAppSimulator.RequestProductPurchaseAsync(productId, false);
#else
                    await CurrentApp.RequestProductPurchaseAsync( productId, false );
#endif
                    if (_licenseInformation.ProductLicenses[productId].IsActive)
                    {
                        //purchase 
                        //TODO: Track
                        return true;
                    }
                    //Check the license state to determine if the in-app purchase was successful.
                }
            }
            catch (Exception)
            {
                // The in-app purchase was not completed because 
                // an error occurred. 
                var localizer = IoC.GetRequiredService<ILocalizationService>();
                MessageDialog dialog = new MessageDialog(localizer.PurchaseUnsuccessfulText, localizer.PurchaseUnsuccessful);
                await dialog.ShowAsync();
            }
            return false;
        }
    }
}
