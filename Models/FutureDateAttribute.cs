using System.ComponentModel.DataAnnotations;

public class FutureDateAttribute : ValidationAttribute
{
    private readonly int _years;
    public FutureDateAttribute(int years) => _years = years;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateValue)
        {
            if (dateValue <= DateTime.Now)
                return new ValidationResult("Date must be later than today.");
            
            if (dateValue > DateTime.Now.AddYears(_years))
                return new ValidationResult($"Date cannot be more than {_years} years in the future.");

            return ValidationResult.Success;
        }
        return new ValidationResult("Invalid date.");
    }
}