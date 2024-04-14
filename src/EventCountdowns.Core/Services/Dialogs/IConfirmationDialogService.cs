using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCountdowns.Core.Services.ConfirmationDialog;

public interface IConfirmationDialogService
{
	Task ShowAsync(string title, string text, Action yesAction, Action noAction);
}
