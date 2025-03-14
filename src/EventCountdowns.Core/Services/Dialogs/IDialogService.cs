using System.Reflection;
using MZikmund.Models.Dialogs;

namespace MZikmund.Services.Dialogs;

public interface IDialogService
{
	Task<ContentDialogResult> ShowAsync(string title, string content);

	Task<ContentDialogResult> ShowAsync(ContentDialog contentDialog);
}
