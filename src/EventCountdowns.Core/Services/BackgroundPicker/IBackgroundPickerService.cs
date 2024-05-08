namespace EventCountdowns.Core.Services.BackgroundPicker;

public interface IBackgroundPickerService
{
	Task<Uri?> PickBackgroundAsync();
}
