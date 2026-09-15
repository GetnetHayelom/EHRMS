using Microsoft.EntityFrameworkCore;
using PIS2.Data;

namespace PIS2.Services
{
   


    public class LanguageContext
    {
        public int LanguageID { get; set; }

        public string LanguageCode { get; set; } = "en";

        public string LanguageName { get; set; } = "English";

        public bool IsRTL { get; set; } = false;

        public int DefaultLanguageID { get; set; }

        public string DefaultLanguageCode { get; set; } = "en";
    }
    public class LocalizationService
    {
        private readonly PISContext _context;
        private readonly LanguageContext _languageContext;

        private Dictionary<string, string> _translations = new();
        private Dictionary<string, string> _defaultTranslations = new();

        private Dictionary<string, string> _entityTranslations = new();
        private Dictionary<string, string> _defaultEntityTranslations = new();

        public LocalizationService(
            PISContext context,
            LanguageContext languageContext)
        {
            _context = context;
            _languageContext = languageContext;
        }

        public async Task InitializeAsync()
        {
            // Current language translations
            _translations = await _context.AppTranslations
                .AsNoTracking()
                .Where(x =>
                    x.languageID == _languageContext.LanguageID &&
                    x.isActive)
                .ToDictionaryAsync(
                    x => x.translationKey,
                    x => x.translationValue);

            // Default language translations
            if (_languageContext.LanguageID != _languageContext.DefaultLanguageID)
            {
                _defaultTranslations = await _context.AppTranslations
                    .AsNoTracking()
                    .Where(x =>
                        x.languageID == _languageContext.DefaultLanguageID &&
                        x.isActive)
                    .ToDictionaryAsync(
                        x => x.translationKey,
                        x => x.translationValue);
            }

            // Entity translations - current language
            _entityTranslations = await _context.EntityTranslations
                .AsNoTracking()
                .Where(x =>
                    x.languageID == _languageContext.LanguageID)
                .ToDictionaryAsync(
                    x => CreateEntityKey(
                        x.entityType,
                        x.entityID,
                        x.propertyName),
                    x => x.translationValue);

            // Entity translations - default language
            if (_languageContext.LanguageID != _languageContext.DefaultLanguageID)
            {
                _defaultEntityTranslations = await _context.EntityTranslations
                    .AsNoTracking()
                    .Where(x =>
                        x.languageID == _languageContext.DefaultLanguageID)
                    .ToDictionaryAsync(
                        x => CreateEntityKey(
                            x.entityType,
                            x.entityID,
                            x.propertyName),
                        x => x.translationValue);
            }
        }

        public string T(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return string.Empty;

            // 1. Current language
            if (_translations.TryGetValue(key, out var translation))
                return translation;

            // 2. Default language
            if (_defaultTranslations.TryGetValue(key, out var defaultTranslation))
                return defaultTranslation;

            // 3. Original key
            return key;
        }

        public string? TE(
            string entityType,
            int entityID,
            string propertyName)
        {
            if (string.IsNullOrWhiteSpace(entityType) ||
                string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }

            var key = CreateEntityKey(
                entityType,
                entityID,
                propertyName);

            // 1. Current language
            if (_entityTranslations.TryGetValue(
                key,
                out var translation))
            {
                return translation;
            }

            // 2. Default language
            if (_defaultEntityTranslations.TryGetValue(
                key,
                out var defaultTranslation))
            {
                return defaultTranslation;
            }

            // 3. No translation exists
            return null;
        }

        private static string CreateEntityKey(
            string entityType,
            int entityID,
            string propertyName)
        {
            return $"{entityType}|{entityID}|{propertyName}";
        }
    }

    public class LocalizationMiddleware
    {
        private readonly RequestDelegate _next;

        public LocalizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext httpContext,
            PISContext context,
            LanguageContext languageContext,
            LocalizationService localizationService)
        {
            // Get language from cookie
            var languageCode =
                httpContext.Request.Cookies["PIS2.Culture"];

            if (string.IsNullOrWhiteSpace(languageCode))
            {
                languageCode = "en";
            }

            // Find requested language
            var language = await context.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.languageCode == languageCode &&
                    x.isActive);

            // Find default language
            var defaultLanguage = await context.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.isDefault &&
                    x.isActive);

            // Safety fallback
            if (defaultLanguage == null)
            {
                defaultLanguage = await context.Languages
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.languageCode == "en" &&
                        x.isActive);
            }

            if (defaultLanguage == null)
            {
                throw new Exception(
                    "No default language has been configured.");
            }

            // If requested language does not exist,
            // use default language.
            language ??= defaultLanguage;

            languageContext.LanguageID = language.languageID;
            languageContext.LanguageCode = language.languageCode;
            languageContext.LanguageName = language.languageName;
            languageContext.IsRTL = language.isRTL;

            languageContext.DefaultLanguageID =
                defaultLanguage.languageID;

            languageContext.DefaultLanguageCode =
                defaultLanguage.languageCode;

            // Load translations
            await localizationService.InitializeAsync();

            await _next(httpContext);
        }
    }
}
