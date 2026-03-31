namespace Awaitick.Services.Theming;

public interface IThemeManager
{
	void SetTheme(ElementTheme theme);

	void SetTitleBarTheme(ElementTheme theme);

	ElementTheme CurrentTheme { get; }

	ApplicationTheme ActualTheme { get; }
}
