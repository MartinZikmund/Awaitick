using Awaitick.Core.ViewModels;

namespace Awaitick.Views;

public sealed partial class CountdownEditorView : CountdownEditorViewBase
{
	private Window? _window;

	public CountdownEditorView()
	{
		this.InitializeComponent();
		Loaded += OnViewLoaded;
		Unloaded += OnViewUnloaded;
	}

	private void OnViewLoaded(object sender, RoutedEventArgs e)
	{
		// Dismiss any open picker flyout when the window is deactivated (e.g. Alt+Tab).
		// Leaving a TimePicker/CalendarDatePicker light-dismiss popup open across a
		// window deactivate/reactivate can freeze or crash the app on Windows (#479).
		_window ??= (Application.Current as CountdownsApp)?.MainWindow;
		if (_window is not null)
		{
			_window.Activated -= OnWindowActivated;
			_window.Activated += OnWindowActivated;
		}
	}

	private void OnViewUnloaded(object sender, RoutedEventArgs e)
	{
		if (_window is not null)
		{
			_window.Activated -= OnWindowActivated;
			_window = null;
		}
	}

	private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
	{
		if (args.WindowActivationState != WindowActivationState.Deactivated)
		{
			return;
		}

		DismissOpenPickerFlyouts();
	}

	private void DismissOpenPickerFlyouts()
	{
		if (XamlRoot is null)
		{
			return;
		}

		try
		{
			var openPopups = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetOpenPopupsForXamlRoot(XamlRoot);
			foreach (var popup in openPopups)
			{
				// Only dismiss transient light-dismiss popups (picker/combo flyouts);
				// never modal popups such as ContentDialog.
				if (popup.IsLightDismissEnabled)
				{
					popup.IsOpen = false;
				}
			}
		}
		catch
		{
			// Never let defensive cleanup throw during deactivation.
		}
	}
}

public abstract partial class CountdownEditorViewBase : PageBase<CountdownEditorViewModel>
{
}
