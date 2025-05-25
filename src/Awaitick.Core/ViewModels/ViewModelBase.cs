#nullable enable

using Awaitick.Core.Infrastructure;
using Awaitick.Core.Services.InAppPurchases;

namespace Awaitick.Core.ViewModels;

public abstract partial class ViewModelBase : ObservableRecipient
{
	[ObservableProperty]
	public partial bool IsWorking { get; set; }
}
