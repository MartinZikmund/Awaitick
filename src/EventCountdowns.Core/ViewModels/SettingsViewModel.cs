using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventCountdowns.Services.Navigation;
using EventCountdowns.ViewModels;

namespace EventCountdowns.Core.ViewModels;

public class SettingsViewModel : PageViewModel
{
	public SettingsViewModel(INavigationService navigationService) : base(navigationService)
	{
	}
}
