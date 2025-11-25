using Backend_Net.Application.Common.Localization;
using Backend_Net.Domain.Enums;

namespace Backend_Net.Application.Common.Extensions
{
    public static class ErrorCodeExtensions
    {
        public static string Localize(this Enum code)
        {
            return LocalizationAccessor.Localizer.GetString(code.ToString());
        }
        
        public static string Code<TTranslation>(this TTranslation message)
        {
            return message.ToString();
        }
    }
}