using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCountdowns.Core.Services.InAppPurchases;

public interface IInAppPurchaseService
{
	bool HasUserAnyProduct();

	Task<bool> PurchaseAsync(InAppProducts product);
}
