using Windows.ApplicationModel.Store;
using Windows.UI.Popups;
<<<<<<<< HEAD:src/Awaitick.Core/Services/InAppPurchases/InAppPurchaseService.cs
using Awaitick.Core.Services.InAppPurchases;
using Awaitick.Core.Infrastructure;
========
using Awaitick.Core.Services.InAppPurchases;
using Awaitick.Core.Infrastructure;
using System.Diagnostics.CodeAnalysis;
>>>>>>>> 365f6b3 (feat: Enhance build settings and improve code quality):src/Awaitick.Core/Services/InAppPurchases/InAppPurchaseService.WinAppSDK.cs

namespace Awaitick.Core.Services;

public class InAppPurchaseService : IInAppPurchaseService
{
	private LicenseInformation? _licenseInformation;

	[MemberNotNull(nameof(_licenseInformation))]
	private void EnsureLicenseInformation()
	{
		if (_licenseInformation == null)
		{
#if DEBUG
			_licenseInformation = CurrentAppSimulator.LicenseInformation;
#else
            _licenseInformation = CurrentApp.LicenseInformation;
#endif
		}

		if (_licenseInformation is null)
		{
			throw new InvalidOperationException("License information is not available.");
		}
	}

	public bool HasUserAnyProduct()
	{
		try
		{
			EnsureLicenseInformation();
			string[] durableIds = new[]
			{
				GetProductId(InAppProducts.SmallCoffee), GetProductId(InAppProducts.MediumCoffee),
				GetProductId(InAppProducts.LargeCoffee), GetProductId(InAppProducts.GigaCoffee)
			};
			return (from d in durableIds where _licenseInformation.ProductLicenses[d].IsActive select d).Any();
		}
		catch (Exception)
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
					return "AwaitickSmallCoffee";
				case InAppProducts.MediumCoffee:
					return "AwaitickMediumCoffee";
				case InAppProducts.LargeCoffee:
					return "AwaitickLargeCoffee";
				case InAppProducts.GigaCoffee:
					return "AwaitickGigaCoffee";
				default:
					throw new ArgumentOutOfRangeException(nameof(product), product, null);
			}
		}
		else
		{
			switch (product)
			{
				case InAppProducts.SmallCoffee:
					return "AwaitickDemiCoffee";
				case InAppProducts.MediumCoffee:
					return "AwaitickGrandeCoffee";
				case InAppProducts.LargeCoffee:
					return "AwaitickVentiCoffee";
				case InAppProducts.GigaCoffee:
					return "AwaitickTrentaCoffee";
				default:
					throw new ArgumentOutOfRangeException(nameof(product), product, null);
			}
		}

	}

	public async Task<bool> PurchaseAsync(InAppProducts product)
	{
		try
		{
			EnsureLicenseInformation();
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
			var localizer = IoC.GetRequiredService<IStringLocalizer>();
			MessageDialog dialog = new MessageDialog(localizer.GetString("PurchaseUnsuccessfulText"), localizer.GetString("PurchaseUnsuccessful"));
			await dialog.ShowAsync();
		}
		return false;
	}
}
