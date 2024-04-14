namespace EventCountdowns.Core.Services.Dialogs;

public interface IDialogService
{
	Task<ContentDialogResult> ShowAsync(string title, string text);
}
