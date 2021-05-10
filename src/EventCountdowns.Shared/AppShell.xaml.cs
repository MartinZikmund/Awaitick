using System;
using EventCountdowns.Core.Infrastructure;
using EventCountdowns.Core.ViewModels;
using EventCountdowns.Models.Theming;
using EventCountdowns.Core.Resources;
using EventCountdowns.Services.Theming;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EventCountdowns
{
    public sealed partial class AppShell : Page
    {
        private static readonly Lazy<AppShell> _instance = new Lazy<AppShell>(() => new AppShell());
        private readonly UISettings _uiSettings = new UISettings();

        public AppShell()
        {
            InitializeComponent();
            ViewModel = IoC.GetRequiredService<AppShellViewModel>();
            _uiSettings.ColorValuesChanged += ColorValuesChanged;
            SetupCoreWindow();

            Loaded += AppShell_Loaded;
        }

        private void AppShell_Loaded(object sender, RoutedEventArgs e)
        {
            SetTitlebarColors();
        }

        public AppShellViewModel ViewModel { get; }

        public Frame RootFrame => InnerFrame;

        public static AppShell GetForCurrentView() => _instance.Value;

        private void SetupCoreWindow()
        {
#pragma warning disable CS8618
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
#pragma warning restore CS8618
        }

        private async void ColorValuesChanged(UISettings sender, object args)
        {
            await Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                SetTitlebarColors);
        }

        private void SetTitlebarColors()
        {
#pragma warning disable CS8618
#pragma warning disable Uno0001
            var brandColor = ColorResources.BrandColor;
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = brandColor;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            if (IoC.GetRequiredService<IThemeManager>().CurrentTheme == AppTheme.Dark)
            {
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonInactiveForegroundColor = Colors.Gray;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(100, 100, 100, 100);
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(200, 100, 100, 100);
                titleBar.ButtonPressedForegroundColor = Colors.White;
            }
            else
            {
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonInactiveForegroundColor = Colors.Gray;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(100, 200, 200, 200);
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(200, 200, 200, 200);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
            }
#pragma warning restore Uno0001
#pragma warning restore CS8618
        }
    }
}
