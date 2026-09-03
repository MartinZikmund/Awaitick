using Awaitick.Core.Models.Licensing;
using Awaitick.Services.Navigation;
using Windows.System;

namespace Awaitick.Core.ViewModels;

public partial class LicensesViewModel : PageViewModel
{
	public LicensesViewModel(INavigationService navigationService, IStringLocalizer stringLocalizer)
		: base(navigationService)
	{
		Title = stringLocalizer.GetString("OpenSourceLicenses");
		AppName = stringLocalizer.GetString("ApplicationName");
	}

	public string AppName { get; }

	public string Copyright => AppLicenseInfo.Copyright;

	public string LicenseName => AppLicenseInfo.LicenseName;

	public string RepositoryUrl => AppLicenseInfo.RepositoryUrl;

	public IReadOnlyList<ThirdPartyNoticeGroup> Packages => ThirdPartyNotices.Packages;

	public IReadOnlyList<ThirdPartyNotice> Assets => ThirdPartyNotices.Assets;

	[RelayCommand]
	private async Task OpenRepositoryAsync() => await Launcher.LaunchUriAsync(new Uri(AppLicenseInfo.RepositoryUrl));

	[RelayCommand]
	private async Task OpenLicenseAsync() => await Launcher.LaunchUriAsync(new Uri(AppLicenseInfo.LicenseUrl));
}
