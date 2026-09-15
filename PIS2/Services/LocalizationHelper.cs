using PIS2.Services;

namespace PIS2.Services
{
    public class LocalizationHelper
    {
        private readonly LocalizationService _localizationService;

        public LocalizationHelper(
            LocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        /// <summary>
        /// Gets a system/application translation.
        /// Falls back to the key if no translation exists.
        /// </summary>
        public string T(string key)
        {
            return _localizationService.T(key);
        }

        /// <summary>
        /// Gets a translated value for a user-defined entity property.
        /// Returns null when no translation exists.
        /// </summary>
        public string? TE(
            string entityType,
            int entityID,
            string propertyName)
        {
            return _localizationService.TE(
                entityType,
                entityID,
                propertyName);
        }

        /// <summary>
        /// Gets a translated value for a user-defined entity property.
        /// Falls back to the original database value.
        /// </summary>
        public string TE(
            string entityType,
            int entityID,
            string propertyName,
            string fallbackValue)
        {
            return _localizationService.TE(
                       entityType,
                       entityID,
                       propertyName)
                   ?? fallbackValue;
        }
    }
}