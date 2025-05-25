using Windows.UI;

namespace Awaitick.Core.ViewModels;

public interface ICountdownEditorViewService
{
	Task<Color?> ShowColorPickerDialog(Color defaultColor);
}
