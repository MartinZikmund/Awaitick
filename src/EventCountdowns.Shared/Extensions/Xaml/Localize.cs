using System;
using EventCountdowns.Core.Infrastructure;
using EventCountdowns.Core.Services;
using Windows.UI.Xaml.Markup;

namespace EventCountdowns.Extensions.Xaml
{
    public class Localize : MarkupExtension
    {
        private static Lazy<ILocalizationService> _localization = new Lazy<ILocalizationService>(
            () => IoC.GetRequiredService<ILocalizationService>());

        public string Key { get; set; }

        protected override object ProvideValue() => _localization.Value.GetString(Key);
    }
}
