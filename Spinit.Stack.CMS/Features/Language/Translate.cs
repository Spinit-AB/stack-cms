using Our.Umbraco.Vorto.Models;

namespace Spinit.Stack.CMS.Features.Language
{
    public static class Translate
    {
        public const string DEFAULT_LANGUAGE = "en-US";

        public static string Value(VortoValue value, string language = null)
        {
            var translation = string.Empty;

            if (language != null)
            {
                translation = value?.Values?[language]?.ToString();

                if (!string.IsNullOrEmpty(translation))
                {
                    return translation;
                }
            }

            translation = value?.Values?[DEFAULT_LANGUAGE]?.ToString();
            if (translation != null)
            {
                return translation;
            }

            return string.Empty;
        }
    }
}