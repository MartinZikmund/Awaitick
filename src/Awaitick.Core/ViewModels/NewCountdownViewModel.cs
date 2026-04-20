using Awaitick.Core.Models.Presets;
using Awaitick.Services.Localization;
using Awaitick.Services.Navigation;

namespace Awaitick.Core.ViewModels;

public partial class NewCountdownViewModel : PageViewModel
{
	private readonly INavigationService _navigationService;
	private readonly IStringLocalizer _localizationService;

	public NewCountdownViewModel(
		INavigationService navigationService,
		IStringLocalizer localizationService) :
		base(navigationService)
	{
		_navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
		_localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
	}

	public IEnumerable<EventPreset> Presets => EventPresets.InDisplayOrder();

	public override void ViewNavigatedTo(object? parameter)
	{
		Title = _localizationService.GetString("NewCountdown_Title");
	}

	[RelayCommand]
	private void SelectPreset(EventPreset? preset)
	{
		if (preset is null)
		{
			return;
		}

		_navigationService.Navigate<CountdownEditorViewModel>(
			CountdownEditorViewModel.NavigationModel.CreateAddFromPreset(preset.Key));
		_navigationService.RemoveLastFromBackStack();
	}

	[RelayCommand]
	private void CreateOwn()
	{
		_navigationService.Navigate<CountdownEditorViewModel>(
			CountdownEditorViewModel.NavigationModel.CreateAdd());
		_navigationService.RemoveLastFromBackStack();
	}
}
