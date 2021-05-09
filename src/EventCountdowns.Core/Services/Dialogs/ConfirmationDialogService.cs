using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using EventCountdowns.Core.Services;
using EventCountdowns.Core.Services.ConfirmationDialog;

namespace EventCountdowns.Core.Services
{
    public class ConfirmationDialogService : IConfirmationDialogService
    {
        private readonly ILocalizationService _localization;

        public ConfirmationDialogService(ILocalizationService localization)
        {
            _localization = localization;
        }

        public async Task ShowAsync(string title, string text, Action yesAction, Action noAction)
        {
            MessageDialog dialog = new MessageDialog(text, title);
            dialog.Commands.Add(new UICommand(_localization.Yes, command => yesAction()));
            dialog.Commands.Add(new UICommand(_localization.No, command => noAction()));
            await dialog.ShowAsync();
        }
    }
}
