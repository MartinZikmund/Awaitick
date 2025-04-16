namespace EventCountdowns.Core.Services;

public interface IImagePickerService
{
	Task<Uri?> PickAsync();
}
