using MZikmund.Toolkit.WinUI.Services;

namespace EventCountdowns.Core.Services.Settings;

public class AppPreferences : IAppPreferences
{
	private readonly IPreferences _settingsService;

	public AppPreferences(IPreferences settingsService)
	{
		_settingsService = settingsService;
	}

	private const string DataVersionKey = "AppDataVersion";

	public int DataVersion
	{
		get => _settingsService.Get(DataVersionKey, 0);
		set => _settingsService.Set(DataVersionKey, value);
	}

	private const string FirstStartKey = "AppFirstStart";

	public bool FirstStart
	{
		get => _settingsService.Get(FirstStartKey, true);
		set => _settingsService.Set(FirstStartKey, value);
	}

	private const string LaunchCountKey = "AppLaunchCount";

	public int LaunchCount
	{
		get => _settingsService.Get(LaunchCountKey, 0);
		set => _settingsService.Set(LaunchCountKey, value);
	}

	private const string OfferUserRatingKey = "OfferUserRating";

	public bool OfferUserRating
	{
		get => _settingsService.Get(OfferUserRatingKey, true);
		set => _settingsService.Set(OfferUserRatingKey, value);
	}

	private const string AppThemeKey = "AppTheme";

	public AppTheme Theme
	{
		get => _settingsService.Get(AppThemeKey, () => AppTheme.System);
		set => _settingsService.Set(AppThemeKey, value);
	}
}
