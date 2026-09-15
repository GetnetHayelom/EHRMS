using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS2.Services;

namespace PIS2.Models
{
    public abstract class BasePageModel : PageModel
    {
        protected readonly LocalizationService Localization;

        protected BasePageModel(
            LocalizationService localization)
        {
            Localization = localization;
        }

        protected string T(string key)
        {
            return Localization.T(key);
        }

        protected string? TE(
            string entityType,
            int entityID,
            string propertyName)
        {
            return Localization.TE(
                entityType,
                entityID,
                propertyName);
        }
    }
}
