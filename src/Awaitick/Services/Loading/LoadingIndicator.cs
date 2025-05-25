using Awaitick.ViewModels;

namespace MZikmund.Services.Loading;

public class LoadingIndicator : ILoadingIndicator
{
	private readonly WindowShellViewModel _windowShellViewModel;

	public LoadingIndicator(WindowShellViewModel windowShellProvider)
	{
		_windowShellViewModel = windowShellProvider ?? throw new ArgumentNullException(nameof(windowShellProvider));
	}

	public IDisposable BeginLoading() => _windowShellViewModel.BeginLoading();

	public bool IsLoading => _windowShellViewModel.IsWorking;

	public string StatusMessage
	{
		get => _windowShellViewModel.LoadingStatusMessage;
		set => _windowShellViewModel.LoadingStatusMessage = value;
	}
}
