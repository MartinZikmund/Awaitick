using Windows.UI;

namespace EventCountdowns.Core.ViewModels;

public interface ICountdownEditorViewService
{
	Task<Color?> ShowColorPickerDialog(Color defaultColor);
}
