using System.Reflection;
using MZikmund.Models.Dialogs;
using MZikmund.Toolkit.WinUI.Infrastructure;
using MZikmund.Toolkit.WinUI.Services;

namespace MZikmund.Services.Dialogs;

public class DialogService : IDialogService
{
	private readonly Dictionary<string, Type> _dialogs = new();
	private readonly IDialogCoordinator _dialogCoordinator;
	private readonly IXamlRootProvider _xamlRootProvider;

	public DialogService(IDialogCoordinator dialogCoordinator, IXamlRootProvider xamlRootProvider)
	{
		_dialogCoordinator = dialogCoordinator ?? throw new ArgumentNullException(nameof(dialogCoordinator));
		_xamlRootProvider = xamlRootProvider ?? throw new ArgumentNullException(nameof(xamlRootProvider));
	}

	public async Task<ContentDialogResult> ShowAsync(string title, string content)
	{
		var dialog = new ContentDialog()
		{
			Title = title,
			Content = content,
			XamlRoot = _xamlRootProvider.XamlRoot
		};

		return await _dialogCoordinator.ShowAsync(dialog);
	}

	public async Task<ContentDialogResult> ShowAsync(ContentDialog contentDialog)
	{
		contentDialog.XamlRoot = _xamlRootProvider.XamlRoot;
		return await _dialogCoordinator.ShowAsync(contentDialog);
	}
}
