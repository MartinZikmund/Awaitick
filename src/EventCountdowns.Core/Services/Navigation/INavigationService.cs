using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace EventCountdowns.Core.Services.Navigation;

public interface INavigationService
{
	void Navigate<TViewModel>();

	void Navigate<TViewModel>(object navigationModel);

	void GoBack();

	bool CanGoBack { get; }

	INavigationService RegisterForNavigation<TViewModel, TPage>()
		where TPage : Page;
}
