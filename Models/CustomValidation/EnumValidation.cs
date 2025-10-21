using System.ComponentModel.DataAnnotations;

namespace Models.CustomValidation {
	public class EnumValidation {
		public static ValidationResult? ValidateEnum<T>(T value, ValidationContext context) where T : struct, Enum {
			// Ensure the value is defined in the FuelType enum
			if (!Enum.IsDefined(value)) {
				return new ValidationResult($"{context.DisplayName} has an invalid value: {value}");
			}
			return ValidationResult.Success;
		}
	}
}
