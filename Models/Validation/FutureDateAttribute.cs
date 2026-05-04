using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Group1Flight.Models.Validation
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
    var attributes = context.Attributes;
    var key = "data-val-futuredate";
    // 'ErrorMessage' is a property of the Attribute class
    var value = ErrorMessage; 

    MergeAttribute(attributes, key, value?.ToString() ?? string.Empty);
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