using Windows.UI.Popups;
using EventCountdowns.Core.Services.ConfirmationDialog;

namespace EventCountdowns.Core.Services;

public class ConfirmationDialogService : IConfirmationDialogService
{
	private readonly IStringLocalizer _localization;

	public ConfirmationDialogService(IStringLocalizer localization)
	{
		_localization = localization;
	}

	public async Task ShowAsync(string title, string text, Action yesAction, Action noAction)
	{
		MessageDialog dialog = new MessageDialog(text, title);
		dialog.Commands.Add(new UICommand(_localization.GetString("Yes"), command => yesAction()));
		dialog.Commands.Add(new UICommand(_localization.GetString("No"), command => noAction()));
		await dialog.ShowAsync();
	}
}
