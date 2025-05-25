namespace Awaitick.Core.ViewModels;

public partial class CountdownEditorViewModel
{
	public class NavigationModel
	{
		public NavigationModel()
		{
		}

		private NavigationModel(string id)
		{
			if (id == null) throw new ArgumentNullException(nameof(id));
			Mode = EditorMode.Edit;
			Id = id;
		}

		public static NavigationModel CreateAdd() => new();

		public static NavigationModel CreateEdit(string id) => new(id);

		public string Id { get; set; } = "";

		public EditorMode Mode { get; set; } = EditorMode.Add;
	}
}
