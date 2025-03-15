#nullable enable

using EventCountdowns.Core.Infrastructure;
using EventCountdowns.Core.Services.InAppPurchases;

namespace EventCountdowns.Core.ViewModels;

public abstract partial class ViewModelBase : ObservableRecipient
{
	[ObservableProperty]
	public partial bool IsWorking { get; set; }
}
