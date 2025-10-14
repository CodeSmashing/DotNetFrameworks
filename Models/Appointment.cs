using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models {
	public class Appointment {
		// Dummy instantie voor referentie
		static public readonly Appointment Dummy = new() { Title = "-", Deleted = DateTime.Now };

		private DateTime now = DateTime.Now + new TimeSpan(0, 1, 0);

		private DateTime _from = DateTime.Now + new TimeSpan(1, 0, 0, 0);

		public long Id {
			get; set;
		}

		[Required]
		[Display(Name = "Vanaf")]
		[DataType(DataType.DateTime)]
		public DateTime From {
			get => _from;
			set {
				if (value < now)
					_from = now;
				else
					_from = value;
			}
		}

		[Required]
		[DataType(DataType.DateTime)]
		[Display(Name = "Tot")]
		public DateTime To { get; set; } = DateTime.Now + new TimeSpan(1, 1, 30, 0);

		[Required]
		[Display(Name = "Titel")]
		public string Title { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Omschrijving")]
		[DataType(DataType.MultilineText)]
		public string Description { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Aangemaakt")]
		public DateTime Created { get; set; } = DateTime.Now;

		[Required]
		[Display(Name = "Verwijderd")]
		public DateTime Deleted { get; set; } = DateTime.MaxValue;

		// Foreign key naar AppointmentType:  Zorg voor de juiste één-op-veel relatie
		[Required]
		[Display(Name = "Type")]
		[ForeignKey("AppointmentType")]
		public int AppointmentTypeId { get; set; } = AppointmentType.Dummy.Id;

		[Required]
		[Display(Name = "IsApproved")]
		public bool IsApproved { get; set; } = false;

		// Navigatie-eigenschap
		public AppointmentType? AppointmentType {
			get; set;
		}

		public override string ToString() {
			return Id + "  Afspraak op " + From + " betreffende " + Title;
		}

		public static List<Appointment> SeedingData() {
			var list = new List<Appointment>();
			list.AddRange(
				// Voeg een default-appointment toe
				Dummy,

				// Voeg enkele test-appointments toe
				new() {
					Title = "Afspraak met Jan",
					Description = "Bespreking van het tuinontwerp",
					From = DateTime.Now.AddDays(2).AddHours(10),
					To = DateTime.Now.AddDays(2).AddHours(11),
					AppointmentTypeId = 2 },

				new() {
					Title = "Onderhoud tuin bij Piet",
					Description = "Jaarlijks onderhoud van de tuin",
					From = DateTime.Now.AddDays(5).AddHours(9),
					To = DateTime.Now.AddDays(5).AddHours(10),
					AppointmentTypeId = 1 },

				new() {
					Title = "Kennismaking met Klaas",
					Description = "Eerste gesprek over mogelijke tuinprojecten",
					From = DateTime.Now.AddDays(7).AddHours(14),
					To = DateTime.Now.AddDays(7).AddHours(15),
					AppointmentTypeId = 3 }
			);

			return list;
		}
	}
}