using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using EventCountdowns.Core.Services;

namespace EventCountdowns.Core.Services
{
    public class LocalizationService : LocalizationServiceBase
    {
        private readonly ResourceLoader _resoruceLoader = ResourceLoader.GetForViewIndependentUse();
        public override string this[string key]
        {
            get
            {
                try
                {
                    return _resoruceLoader.GetString(key);
                }
                catch (Exception)
                {
                    return key;
                }
            }
        }
    }
}
