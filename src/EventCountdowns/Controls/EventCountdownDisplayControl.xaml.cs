using EventCountdowns.Core.Models;

namespace EventCountdowns.Controls;

public sealed partial class EventCountdownDisplayControl : UserControl
{
	public EventCountdownDisplayControl()
	{
		this.InitializeComponent();
		this.Loaded += EventCountdownDisplayControl_Loaded;
	}

	private void EventCountdownDisplayControl_Loaded(object sender, RoutedEventArgs e)
	{
		UpdateDisplay();
	}

	public CountdownViewModel Countdown
	{
		get => (CountdownViewModel)GetValue(CountdownProperty);
		set => SetValue(CountdownProperty, value);
	}

	public static DependencyProperty CountdownProperty { get; } =
		DependencyProperty.Register(
			nameof(Countdown),
			typeof(CountdownViewModel),
			typeof(EventCountdownDisplayControl),
			new PropertyMetadata(null, OnCountdownChanged));

	private static void OnCountdownChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
	{
		if (dependencyObject is not EventCountdownDisplayControl control || args.NewValue is not CountdownViewModel countdown)
		{
			return;
		}

		control.RootGrid.RequestedTheme = countdown.Theme;
	}

	public CountdownDisplayMode DisplayMode
	{
		get => (CountdownDisplayMode)GetValue(DisplayModeProperty);
		set => SetValue(DisplayModeProperty, value);
	}

	public static DependencyProperty DisplayModeProperty { get; } =
		DependencyProperty.Register(nameof(DisplayMode), typeof(CountdownDisplayMode), typeof(EventCountdownDisplayControl), new PropertyMetadata(CountdownDisplayMode.Full, OnDisplayModeChanged));

	private static void OnDisplayModeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
	{
		if (dependencyObject is not EventCountdownDisplayControl control)
		{
			return;
		}

		control.UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		BottomInfo.Visibility = DisplayMode == CountdownDisplayMode.Full ? Visibility.Visible : Visibility.Collapsed;
	}
}
