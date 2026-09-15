using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class LanguageModel
    {
        [Key]
        public int languageID { get; set; }

        [Required]
        [StringLength(10)]
        public string languageCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string languageName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? nativeName { get; set; }

        public bool isRTL { get; set; } = false;

        public bool isActive { get; set; } = true;

        public bool isDefault { get; set; } = false;

        // Navigation
        public virtual ICollection<AppTranslationModel> AppTranslations { get; set; }
            = new List<AppTranslationModel>();

        public virtual ICollection<EntityTranslationModel> EntityTranslations { get; set; }
            = new List<EntityTranslationModel>();
    }


    public class AppTranslationModel
    {
        [Key]
        public int translationID { get; set; }

        [Required]
        [StringLength(200)]
        public string translationKey { get; set; } = string.Empty;

        public int languageID { get; set; }

        [Required]
        public string translationValue { get; set; } = string.Empty;

        [StringLength(100)]
        public string? module { get; set; }

        [StringLength(500)]
        public string? description { get; set; }

        public bool isActive { get; set; } = true;

        // Navigation
        public virtual LanguageModel? Language { get; set; }
    }


    public class EntityTranslationModel
    {
        [Key]
        public int entityTranslationID { get; set; }

        [Required]
        [StringLength(100)]
        public string entityType { get; set; } = string.Empty;

        public int entityID { get; set; }

        [Required]
        [StringLength(100)]
        public string propertyName { get; set; } = string.Empty;

        public int languageID { get; set; }

        [Required]
        public string translationValue { get; set; } = string.Empty;

        public virtual LanguageModel? Language { get; set; }
    }
}
