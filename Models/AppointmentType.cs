using System.ComponentModel.DataAnnotations;

namespace Models {
	public class AppointmentType {
		// Dummy/default appointment type
		static public readonly AppointmentType Dummy = new() { Name = "-", Description = "-", Deleted = DateTime.MaxValue };

		public int Id {
			get; set;
		}

		[Required(ErrorMessage = "{0} is vereist")]
		[StringLength(25, MinimumLength = 3, ErrorMessage = "De afspraak type naam moet minstens 3 characters en mag maximum 25 characters bevatten")]
		[Display(Name = "Afspraak type naam")]
		public string Name {
			get; set;
		} = string.Empty;

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

		[Required(ErrorMessage = "{0} is vereist")]
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
				// Voeg een dummy AppointmentType toe
				Dummy,

				// Voeg enkele voorbeeld AppointmentTypes toe
				new() {
					Name = "Onderhoud",
					Description = "Algemeen onderhoud"},
				new() {
					Name = "Aanleg",
					Description = "Aanleggen van tuin" },
				new() {
					Name = "Kennismaking",
					Description = "Kennismaking klant" }
			};
		}
	}
}