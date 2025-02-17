    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;
namespace PIS2.Models
{


    public class NumericOnlyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Allow null for optional fields
            }

            var stringValue = value as string;
            if (stringValue != null && Regex.IsMatch(stringValue, @"^\d+$"))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("The field must contain only numeric characters.");
        }
    }

}
