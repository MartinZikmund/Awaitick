#if HAS_UNO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awaitick.Services.Store;

public class ProStoreService : IStoreService
{
	public Task<string?> GetPriceAsync() => Task.FromResult("Nada");
	public Task<bool> HasProAsync() => Task.FromResult(true);
	public Task<bool> TryPurchaseProAsync() => Task.FromResult(true);
}
#endif
