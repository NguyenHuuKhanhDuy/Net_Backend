using Backend_Net.Application.Common.Interfaces;
using Microsoft.Extensions.Localization;

namespace Backend_Net.Infrastructure.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizer _localizer;

        public LocalizationService(IStringLocalizerFactory factory)
        {
            var type = typeof(LocalizationService);
            var assemblyName = type.Assembly.GetName().Name;

            _localizer = factory.Create("Messages", assemblyName!);
        }

        public string GetString(string key)
        {
            var value = _localizer[key];

            if (value.ResourceNotFound)
                return key;

            return value;
        }
    }
}