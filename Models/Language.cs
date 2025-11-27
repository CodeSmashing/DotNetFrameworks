using System.ComponentModel.DataAnnotations;

namespace Models {
	public class Language {
		public static readonly List<Language> Languages = [];
		public static Language Dummy;

		[Key]
		[Display(Name = "Taal code")]
		public required string Code {
			get; set;
		}

		[Display(Name = "Naam")]
		public required string Name {
			get; set;
		}

		[Display(Name = "Is systeem taal")]
		public required bool IsSystemLanguage {
			get; set;
		}

		[Display(Name = "Is actief")]
		public bool IsActive {
			get; set;
		} = false;

		public static Language[] SeedingData() {
			Dummy = new() {
				Code = "-",
				Name = "Dummy",
				IsSystemLanguage = false,
			};

			return [
				// Add a dummy Language
				Dummy,
				
				// Add a few Languages
				new Language() {
					Code = "en",
					Name = "English",
					IsSystemLanguage = true,
					IsActive = true
				},
				new Language() {
					Code = "nl",
					Name = "Nederlands",
					IsSystemLanguage = true,
					IsActive = true
				},
				new Language() {
					Code = "fr",
					Name = "Français",
					IsSystemLanguage = true,
					IsActive = true
				},
				new Language() {
					Code = "es",
					Name = "Espanol",
					IsSystemLanguage = false
				},
				new Language() {
					Code = "de",
					Name = "Deutch",
					IsSystemLanguage = false
				}
			];
		}
	}
}
