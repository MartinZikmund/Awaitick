using EventCountdowns.Extensions;
using EventCountdowns.Services.Localization;
using EventCountdowns.Services.Navigation;
using EventCountdowns.ViewModels;

namespace EventCountdowns.Services.Dialogs;
public sealed partial class ProOnlyFeatureDialog : ContentDialog
{
	public ProOnlyFeatureDialog()
	{
		this.InitializeComponent();

		UpdateDescription();
	}

	private void UpdateDescription()
	{
		var description = Localizer.Instance.GetString(DescriptionKey);
		DescriptionTextBlock.Text = description;
	}

	public string DescriptionKey
	{
		get => (string)GetValue(DescriptionKeyProperty);
		set => SetValue(DescriptionKeyProperty, value);
	}

	public static DependencyProperty DescriptionKeyProperty { get; } =
		DependencyProperty.Register(nameof(DescriptionKey), typeof(string), typeof(ProOnlyFeatureDialog), new PropertyMetadata("ProOnlyFeatureDescription", OnDescriptionChanged));

	private static void OnDescriptionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) => ((ProOnlyFeatureDialog)sender).UpdateDescription();

	private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
	{
		sender.GetServiceProvider()?.GetRequiredService<INavigationService>().Navigate<GetProViewModel>();
	}
}
