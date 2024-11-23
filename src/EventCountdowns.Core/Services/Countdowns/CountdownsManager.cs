using System.Globalization;
using CommunityToolkit.Mvvm.Messaging;
using EventCountdowns.Core.Messages;
using EventCountdowns.Core.Models;
using EventCountdowns.Core.Services.EventCountdownManager;
using EventCountdowns.Core.ViewModels;
using EventCountdowns.Services.Navigation;
using MZikmund.Services.Dialogs;
using MZikmund.Toolkit.WinUI.Infrastructure;

namespace EventCountdowns.Core.Services.Countdowns;

public class CountdownsManager : ICountdownsManager
{
	private readonly IDialogService _dialogService;
	private readonly INavigationService _navigationService;
	private readonly ISystemSharingService _sharingService;
	private readonly IStringLocalizer _localizer;
	private readonly ICountdownsDataService _countdownsDataService;
	private readonly IMessenger _messenger;
	private readonly IXamlRootProvider _xamlRootProvider;

	public CountdownsManager(
		IDialogService dialogService,
		INavigationService navigationService,
		ISystemSharingService sharingService,
		IStringLocalizer localizer,
		ICountdownsDataService countdownsDataService,
		IMessenger messenger,
		IXamlRootProvider xamlRootProvider)
	{
		_dialogService = dialogService;
		_navigationService = navigationService;
		_sharingService = sharingService;
		_localizer = localizer;
		_countdownsDataService = countdownsDataService;
		_messenger = messenger;
		_xamlRootProvider = xamlRootProvider;
	}

	public async Task<bool> DeleteAsync(CountdownViewModel countdown)
	{
		var confirmationDialog = new ContentDialog
		{
			Title = _localizer.GetString("ConfirmDelete"),
			Content = _localizer.GetString("AreYouSureDeleteTextFormat"),
			PrimaryButtonText = _localizer.GetString("Delete"),
			CloseButtonText = _localizer.GetString("Cancel"),
			XamlRoot = _xamlRootProvider.XamlRoot
		};

		var result = await _dialogService.ShowAsync(confirmationDialog);
		if (result != ContentDialogResult.Primary)
		{
			return false;
		}

		await _countdownsDataService.DeleteCountdownAsync(countdown.Model);
		_messenger.Send(new CountdownDeletedMessage(countdown.Id));
		return true;
	}

	public void GoToEdit(CountdownViewModel eventCountdown)
	{
		_navigationService.Navigate<CountdownEditorViewModel>(new CountdownEditorViewModel.NavigationModel() { Id = eventCountdown.Id, Mode = CountdownEditorViewModel.EditorMode.Edit });
	}

	public async Task ShareAsync(CountdownViewModel countdown)
	{
		string sharedText = "";
		if (countdown.HasFinished)
		{
			sharedText = string.Format(
				CultureInfo.CurrentCulture,
				_localizer.GetString("SharingFinishedEventFormatString"),
				countdown.CelebrationMessage,
				_localizer.GetString("AppName"));
		}
		else
		{
			sharedText = string.Format(
				CultureInfo.CurrentCulture,
				_localizer.GetString("SharingFormatString"),
				countdown.Name,
				countdown.DaysLeft,
				countdown.HoursLeft,
				countdown.MinutesLeft,
				countdown.TargetDateTime.ToString("g", CultureInfo.CurrentCulture),
				_localizer.GetString("AppName"));
		}

		await _sharingService.ShareTextAsync(sharedText);
	}
}
