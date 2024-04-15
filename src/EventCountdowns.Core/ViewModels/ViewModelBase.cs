#nullable enable

using System.Runtime.CompilerServices;
using EventCountdowns.Core.Services.Navigation;
using EventCountdowns.Core.Infrastructure;
using EventCountdowns.Core.Services.InAppPurchases;
using EventCountdowns.Services.Navigation;

namespace EventCountdowns.Core.ViewModels;

public abstract partial class ViewModelBase : ObservableRecipient
{
	[ObservableProperty]
	private bool _isWorking;

	[ObservableProperty]
	private string _title = "";

	public bool UserPremium => IoC.GetRequiredService<IInAppPurchaseService>().HasUserAnyProduct();
}
