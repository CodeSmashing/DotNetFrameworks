using Models.CustomValidation;
using Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models {
	public class AppointmentType {
		public int Id {
			get; set;
		}

		[Required(ErrorMessage = "{0} is vereist")]
		[CustomValidation(typeof(EnumValidation), nameof(EnumValidation.ValidateEnum))]
		[Display(Name = "Afspraak type naam")]
		public AppointmentTypeName Name {
			get; set;
		}

		[Required(ErrorMessage = "{0} is vereist")]
		[StringLength(2000, MinimumLength = 3, ErrorMessage = "De omschrijving moet minstens 3 characters en mag maximum 2000 characters bevatten")]
		[Display(Name = "Omschrijving")]
		public string Description {
			get; set;
		} = string.Empty;

		// Likely to be removed
		public string Color {
			get; set;
		} = "#FF000000"; // Color in which the appointment wil be shown, default is black

		[Display(Name = "Verwijderd")]
		[DataType(DataType.DateTime)]
		public DateTime Deleted {
			get; set;
		} = DateTime.MaxValue;

		public override string ToString() {
			return $"{Id}: {Name} ({Description}) - Deleted: {Deleted}";
		}

		// Seeding data
		public static List<AppointmentType> SeedingData() {
			return new() {
				new() {
					Name = AppointmentTypeName.Onderhoud,
					Description = "Algemeen onderhoud"},
				new() {
					Name = AppointmentTypeName.Aanleg,
					Description = "Aanleggen van tuin" },
				new() {
					Name = AppointmentTypeName.Kennismaking,
					Description = "Kennismaking klant" }
			};
		}
	}
}