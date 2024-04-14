using System.Reflection;

namespace MZikmund.Services.Dialogs;

public interface IDialogService
{
	Task<ContentDialogResult> ShowStatusMessageAsync(
		StatusMessageDialogType type,
		string title,
		string text);

	Task<ContentDialogResult> ShowAsync<TViewModel>(TViewModel viewModel);

	void RegisterDialogsFromAssembly(Assembly assembly);
}
