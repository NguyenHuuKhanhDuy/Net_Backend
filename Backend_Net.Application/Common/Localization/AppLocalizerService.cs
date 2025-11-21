using Microsoft.Extensions.Localization;

namespace Backend_Net.Application.Common.Localization;

public class AppLocalizerService : IAppLocalizer
{
    private readonly IStringLocalizer _localizer;

    public AppLocalizerService(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
    }

    public string this[string key] => _localizer[key];
}