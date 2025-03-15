using EventCountdowns.Services.Navigation;
using Windows.ApplicationModel.DataTransfer;

namespace EventCountdowns.Core.Services;

public class SystemSharingService : ISystemSharingService
{
	private readonly IStringLocalizer _localizationService;
	private readonly IWindowShellProvider _windowShellProvider;

	private string? _data;
	private TaskCompletionSource? _completionSource;

	public SystemSharingService(IStringLocalizer localizationService, IWindowShellProvider windowShellProvider)
	{
		_localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
		_windowShellProvider = windowShellProvider ?? throw new ArgumentNullException(nameof(windowShellProvider));
	}

	public async Task ShareTextAsync(string data)
	{
		_data = data;
		_completionSource = new TaskCompletionSource();

#if HAS_UNO
		ShareUno();
#else
		ShareWinUI3();
#endif
		await _completionSource.Task;
		_data = null;
		_completionSource = null;
	}

	private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
	{
		sender.DataRequested -= OnDataRequested;
		args.Request.Data.SetText(_data ?? "");
		args.Request.Data.Properties.Title = _localizationService.GetString("AppName");
	}

#if HAS_UNO
	private void ShareUno()
	{
		var manager = DataTransferManager.GetForCurrentView();
		manager.DataRequested += OnDataRequested;
		try
		{
			DataTransferManager.ShowShareUI();
			_completionSource?.SetResult();
		}
		catch (Exception ex)
		{
			_completionSource?.SetException(ex);
		}
	}
#else
	private void ShareWinUI3()
	{
		var interop = DataTransferManager.As<IDataTransferManagerInterop>();

		var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(_windowShellProvider.Window);
		IntPtr result = interop.GetForWindow(windowHandle, DataTransferManagerInteropIId);

		var manager = WinRT.MarshalInterface<DataTransferManager>.FromAbi(result);
		manager.DataRequested += OnDataRequested;

		try
		{
			interop.ShowShareUIForWindow(windowHandle);
			_completionSource?.SetResult();
		}
		catch (Exception ex)
		{
			_completionSource?.SetException(ex);
		}
	}

	[System.Runtime.InteropServices.ComImport]
	[System.Runtime.InteropServices.Guid("3A3DCD6C-3EAB-43DC-BCDE-45671CE800C8")]
	[System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)]
	interface IDataTransferManagerInterop
	{
		IntPtr GetForWindow([System.Runtime.InteropServices.In] IntPtr appWindow, [System.Runtime.InteropServices.In] ref Guid riid);

		void ShowShareUIForWindow(IntPtr appWindow);
	}

	private static readonly Guid DataTransferManagerInteropIId = new(0xa5caee9b, 0x8708, 0x49d1, 0x8d, 0x36, 0x67, 0xd2, 0x5a, 0x8d, 0xa0, 0x0c);
#endif
}
