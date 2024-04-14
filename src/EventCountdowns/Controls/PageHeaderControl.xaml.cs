namespace EventCountdowns.Controls;

public sealed partial class PageHeaderControl : UserControl
{
	public PageHeaderControl()
	{
		InitializeComponent();
	}

	public string Title
	{
		get => (string)GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	public static DependencyProperty TitleProperty { get; } =
		DependencyProperty.Register(
			nameof(Title),
			typeof(string),
			typeof(PageHeaderControl),
			new PropertyMetadata(""));
}
