using Models.CustomValidation;
using Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models {
	public class AppointmentType {
		static public readonly AppointmentType Dummy = new() {
			Created = new DateTime(2000, 1, 1),
			Description = string.Empty,
			Name = AppointmentTypeName.Kennismaking
		};

		public string Id {
			get; private set;
		} = Guid.NewGuid().ToString();

		[CustomValidation(typeof(EnumValidation), nameof(EnumValidation.ValidateEnum))]
		[Display(Name = "Afspraak type naam")]
		public required AppointmentTypeName Name {
			get; set;
		}

		[StringLength(2000, MinimumLength = 3, ErrorMessage = "De omschrijving moet minstens 3 characters en mag maximum 2000 characters bevatten")]
		[Display(Name = "Omschrijving")]
		public required string Description {
			get; set;
		}

		// Likely to be removed
		public string Color {
			get; set;
		} = "#FF000000"; // Color in which the appointment wil be shown, default is black

		[Display(Name = "Aangemaakt")]
		[DataType(DataType.DateTime)]
		public DateTime Created {
			get; private set;
		} = DateTime.Now;

		[Display(Name = "Verwijderd")]
		[DataType(DataType.DateTime)]
		public DateTime? Deleted {
			get; set;
		}

		public override string ToString() {
			return $"{Id}: {Name} ({Description}) - Deleted: {Deleted}";
		}

		public static AppointmentType[] SeedingData() {
			return [
				// Add a dummy AppointmentType
				Dummy,
				
				// Add a few example AppointmentTypes
				new() {
					Name = AppointmentTypeName.Onderhoud,
					Description = "Algemeen onderhoud"
				},
				new() {
					Name = AppointmentTypeName.Aanleg,
					Description = "Aanleggen van tuin"
				},
				new() {
					Name = AppointmentTypeName.Kennismaking,
					Description = "Kennismaking klant"
				}
			];
		}
	}
}