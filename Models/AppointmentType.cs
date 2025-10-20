using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models {
	public class AppointmentType {
		// Dummy default user
		static public readonly AppointmentType Dummy = new() { Name = "-", Description = "-", Deleted = DateTime.MaxValue };

		// [Key]
		public int Id {
			get; set;
		}

		[Required]
		public string Name { get; set; } = string.Empty;

		[Required]
		public string Description { get; set; } = string.Empty;

		public string Color { get; set; } = "#FF000000"; // Color in which the appointment wil be shown, default is black

		[Required]
		public DateTime Deleted { get; set; } = DateTime.MaxValue;

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