namespace Awaitick.Core.Services;

public interface IImagePickerService
{
	Task<Uri?> PickAsync();
}
