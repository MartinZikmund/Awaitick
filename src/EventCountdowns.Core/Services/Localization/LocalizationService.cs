using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;

namespace EventCountdowns.Core.Services
{
    public class LocalizationService : LocalizationServiceBase
    {
		private static ResourceLoader? _resourceLoader = null;

		public override string GetString(string key)
		{
			_resourceLoader ??= ResourceLoader.GetForViewIndependentUse();
			var result = _resourceLoader.GetString(key);
			return !string.IsNullOrEmpty(result) ? result : $"???{key}???";
		}

		public override string this[string key] => GetString(key);
	}
}
