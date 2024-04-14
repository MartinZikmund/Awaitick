using Windows.ApplicationModel.DataTransfer;
using EventCountdowns.Core.Services.Share;

namespace EventCountdowns.Core.Services;

public class SystemSharingService : ISystemSharingService
{
	private readonly IStringLocalizer _localizationService;

	private string _data;

	public SystemSharingService(IStringLocalizer localizationService)
	{
		_localizationService = localizationService;
	}

	public Task ShareTextAsync(string data)
	{
		_data = data;
		DataTransferManager.GetForCurrentView().DataRequested += SystemSharingService_DataRequested;
		DataTransferManager.ShowShareUI();
		return Task.CompletedTask;
	}

	private void SystemSharingService_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
	{
		DataTransferManager.GetForCurrentView().DataRequested -= SystemSharingService_DataRequested;
		args.Request.Data.SetText(_data);
		args.Request.Data.Properties.Title = _localizationService.GetString("AppName");
	}
}
