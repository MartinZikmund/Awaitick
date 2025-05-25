namespace Awaitick.Controls;

public sealed partial class PageHeaderControl : UserControl
{
	public PageHeaderControl()
	{
		InitializeComponent();
	}

	public ICommand GoBackCommand
	{
		get => (ICommand)GetValue(GoBackCommandProperty);
		set => SetValue(GoBackCommandProperty, value);
	}

	public static readonly DependencyProperty GoBackCommandProperty =
		DependencyProperty.Register(nameof(GoBackCommand), typeof(ICommand), typeof(PageHeaderControl), new PropertyMetadata(null));

	public bool CanGoBack
	{
		get => (bool)GetValue(CanGoBackProperty);
		set => SetValue(CanGoBackProperty, value);
	}

	public static readonly DependencyProperty CanGoBackProperty =
		DependencyProperty.Register(nameof(CanGoBack), typeof(bool), typeof(PageHeaderControl), new PropertyMetadata(false));

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
