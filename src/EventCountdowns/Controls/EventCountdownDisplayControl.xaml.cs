using EventCountdowns.Core.Models;

namespace EventCountdowns.Controls;

public sealed partial class EventCountdownDisplayControl : UserControl
{
	public EventCountdownDisplayControl()
	{
		this.InitializeComponent();
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
			new PropertyMetadata(null));
}
