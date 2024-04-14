namespace EventCountdowns.Core.Services.InAppPurchases;

public interface IInAppPurchaseService
{
	bool HasUserAnyProduct();

	Task<bool> PurchaseAsync(InAppProducts product);
}
