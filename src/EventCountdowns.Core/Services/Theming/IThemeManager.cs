using EventCountdowns.Models.Theming;

namespace EventCountdowns.Services.Theming
{
    public interface IThemeManager
    {
        void SetTheme(AppTheme theme);

        AppTheme CurrentTheme { get; }
    }
}
