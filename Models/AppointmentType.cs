using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models {
	public class AppointmentType {
		// Dummy instantie voor referentie
		static public readonly AppointmentType Dummy = new() { Name = "-", Description = "-", Deleted = DateTime.MaxValue };

		// [Key]
		public int Id {
			get; set;
		}

		[Required]
		public string Name { get; set; } = string.Empty;

		[Required]
		public string Description { get; set; } = string.Empty;

		[Required]
		public DateTime Deleted { get; set; } = DateTime.MaxValue;

		public override string ToString() {
			return $"{Id}: {Name} ({Description}) - Deleted: {Deleted}";
		}

		// seeding data
		public static List<AppointmentType> SeedingData() {
			List<AppointmentType> list = new();

			list.AddRange(list = new() {
				// Voeg een dummy AppointmentType toe
				Dummy,

				// Voeg enkele voorbeeld AppointmentTypes toe
				new() { Name = "Onderhoud", Description = "Algemeen onderhoud"},
				new() { Name = "Aanleg", Description = "Aanleggen van tuin" },
				new() { Name = "Kennismaking", Description = "Kennismaking klant" }
			});

			return list;
		}
	}
}