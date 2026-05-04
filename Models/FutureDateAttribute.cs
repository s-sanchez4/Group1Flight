using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Group1Flight.Models
{
    public class FutureDateAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly int _maxYears;

        public FutureDateAttribute(int maxYears)
        {
            _maxYears = maxYears;
            // This ensures the error message is available for the "buoying up" process
            ErrorMessage = $"Date cannot be more than {_maxYears} years in the future.";
        }

        // Server-Side Validation (The "Buoying Up" part)
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateValue)
            {
                if (dateValue > DateTime.Now.AddYears(_maxYears))
                {
                    // Returning a specific ValidationResult ensures the controller sees the error
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }

        // Client-Side Validation (The IClientModelValidator implementation)
        public void AddValidation(ClientModelValidationContext context)
        {
            // This adds HTML attributes like data-val-futuredate to your <input> tag
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-futuredate", ErrorMessage);
            MergeAttribute(context.Attributes, "data-val-futuredate-maxyears", _maxYears.ToString());
        }

        private static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }
    }
}