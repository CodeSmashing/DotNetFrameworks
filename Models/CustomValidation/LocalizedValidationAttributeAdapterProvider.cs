using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace Models.CustomValidation {
	/// <summary>
	/// This logic is sourced from a Stack Overflow post to handle error message localization logic.<br/><br/>
	///
	/// Source: <see href="https://stackoverflow.com/a/50230263"/><br/>
	/// Author: Anders, licensed CC BY-SA 4.0<br/>
	/// Retrieved: 2025-12-04<br/>
	/// </summary>
	public class LocalizedValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider {
		private readonly ValidationAttributeAdapterProvider _originalProvider = new();

		public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer) {
			attribute.ErrorMessage = attribute.GetType().Name.Replace("Attribute", string.Empty);
			if (attribute is DataTypeAttribute dataTypeAttribute)
				attribute.ErrorMessage += "_" + dataTypeAttribute.DataType;

			return _originalProvider.GetAttributeAdapter(attribute, stringLocalizer);
		}
	}
}
