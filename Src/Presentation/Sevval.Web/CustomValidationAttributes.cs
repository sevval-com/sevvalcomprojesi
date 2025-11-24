using System.ComponentModel.DataAnnotations;

public class CustomRequiredAttribute : ValidationAttribute
{
    public CustomRequiredAttribute()
    {
        // Varsayılan hata mesajı
        ErrorMessage = "{0} alanı gereklidir.";
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
        {
            // Hata mesajını özelleştirme
            var errorMessage = string.Format(ErrorMessage, validationContext.DisplayName);
            return new ValidationResult(errorMessage);
        }

        return ValidationResult.Success;
    }
}
