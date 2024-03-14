using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace EventCountdowns.Core.Services.Dialogs
{
    public interface IDialogService
    {
        Task<ContentDialogResult> ShowAsync(string title, string text);
    }
}
