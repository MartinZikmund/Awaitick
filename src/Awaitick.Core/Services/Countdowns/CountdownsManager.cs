using System.Globalization;
using Awaitick.Core.Configuration;
using Awaitick.Core.Messages;
using Awaitick.Core.Models;
using Awaitick.Core.Services.Data;
using Awaitick.Core.Services.EventCountdownManager;
using Awaitick.Core.ViewModels;
using Awaitick.Services.Navigation;
using Awaitick.Services.Store;
using Awaitick.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using MZikmund.Services.Dialogs;
using MZikmund.Toolkit.WinUI.Infrastructure;

namespace Awaitick.Core.Services.Countdowns;

public class CountdownsManager : ICountdownsManager
{
	private readonly IDialogService _dialogService;
	private readonly INavigationService _navigationService;
	private readonly ISystemSharingService _sharingService;
	private readonly IStringLocalizer _localizer;
	private readonly ICountdownsDataService _countdownsDataService;
	private readonly IDataService _dataService;
	private readonly IStoreService _storeService;
	private readonly IMessenger _messenger;
	private readonly IXamlRootProvider _xamlRootProvider;

	public CountdownsManager(
		IDialogService dialogService,
		INavigationService navigationService,
		ISystemSharingService sharingService,
		IStringLocalizer localizer,
		ICountdownsDataService countdownsDataService,
		IDataService dataService,
		IStoreService storeService,
		IMessenger messenger,
		IXamlRootProvider xamlRootProvider)
	{
		_dialogService = dialogService;
		_navigationService = navigationService;
		_sharingService = sharingService;
		_localizer = localizer;
		_countdownsDataService = countdownsDataService;
		_dataService = dataService;
		_storeService = storeService;
		_messenger = messenger;
		_xamlRootProvider = xamlRootProvider;
	}

	public async Task<bool> DeleteAsync(CountdownViewModel countdown)
	{
		var confirmationDialog = new ContentDialog
		{
			Title = _localizer.GetString("ConfirmDelete"),
			Content = string.Format(_localizer.GetString("AreYouSureDeleteTextFormat"), countdown.Name),
			PrimaryButtonText = _localizer.GetString("Delete"),
			CloseButtonText = _localizer.GetString("Cancel"),
			DefaultButton = ContentDialogButton.Primary,
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

	public void EditAsync(CountdownViewModel eventCountdown) => _navigationService.Navigate<CountdownEditorViewModel>(new CountdownEditorViewModel.NavigationModel() { Id = eventCountdown.Id, Mode = CountdownEditorViewModel.EditorMode.Edit });

	public void GoToDetail(CountdownViewModel countdown) => _navigationService.Navigate<CountdownDetailViewModel>(new CountdownDetailViewModel.NavigationModel(countdown.Id));

	public void GoToEdit(CountdownViewModel countdown) => _navigationService.Navigate<CountdownEditorViewModel>(new CountdownEditorViewModel.NavigationModel() { Id = countdown.Id, Mode = CountdownEditorViewModel.EditorMode.Edit });

	public async Task DuplicateAsync(CountdownViewModel countdown)
	{
		// Respect the free-tier limit, consistent with adding a new countdown.
		if (!await _storeService.HasProAsync())
		{
			var existing = await _dataService.GetCountdownsAsync();
			if (existing.Count >= FreeTierLimits.MaxCountdowns)
			{
				_navigationService.Navigate<GetProViewModel>();
				return;
			}
		}

		var source = countdown.Model;
		EventCountdown duplicate = new()
		{
			Name = $"{source.Name} (copy)",
			TargetDateTime = source.TargetDateTime,
			BackgroundColor = source.BackgroundColor,
			TextTheme = source.TextTheme,
			BackgroundImageOpacity = source.BackgroundImageOpacity,
			CelebrationMessage = source.CelebrationMessage,
			// Keep built-in (ms-appx) backgrounds, but drop custom/picked images.
			BackgroundImageUri = source.BackgroundImageUri is { } uri && uri.Scheme == "ms-appx" ? uri : null,
		};

		await _countdownsDataService.AddCountdownAsync(duplicate);
		_messenger.Send(new CountdownAddedMessage(duplicate.Id));
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
				countdown.TargetDateString,
				_localizer.GetString("AppName"));
		}

		await _sharingService.ShareTextAsync(sharedText);
	}
}
