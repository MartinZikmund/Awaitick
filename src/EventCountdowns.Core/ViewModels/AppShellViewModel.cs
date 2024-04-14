using Windows.UI.Core;
using Microsoft.UI.Xaml;

namespace EventCountdowns.Core.ViewModels;

public class AppShellViewModel : ViewModel
{
	public Visibility BackButtonVisibility { get; set; } = Visibility.Visible;
}
