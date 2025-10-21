using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models {
	public class Appointment {
		// Dummy/default appointment
		static public readonly Appointment Dummy = new() { Title = "-", Deleted = DateTime.Now };

		private DateTime now = DateTime.Now + new TimeSpan(0, 1, 0);

		private DateTime _from = DateTime.Now + new TimeSpan(1, 0, 0, 0);

		public long Id {
			get; set;
		}

		[Required(ErrorMessage = "{0} is vereist")]
		[ForeignKey("AgendaUser")]
		[Display(Name = "User ID")]
		public string UserId {
			get; set;
		} = AgendaUser.Dummy.Id;

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "User")]
		public AgendaUser AgendaUser {
			get; set;
		} = null!;

		[Required(ErrorMessage = "{0} is vereist")]
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

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Tot")]
		[DataType(DataType.DateTime)]
		public DateTime To {
			get; set;
		} = DateTime.Now + new TimeSpan(1, 1, 30, 0);

		[Required(ErrorMessage = "{0} is vereist")]
		[StringLength(50, MinimumLength = 3, ErrorMessage = "De titel moet minstens 3 characters en mag maximum 50 characters bevatten")]
		[Display(Name = "Titel")]
		public string Title {
			get; set;
		} = string.Empty;

		[Required(ErrorMessage = "{0} is vereist")]
		[StringLength(2000, MinimumLength = 3, ErrorMessage = "De omschrijving moet minstens 3 characters en mag maximum 2000 characters bevatten")]
		[Display(Name = "Omschrijving")]
		[DataType(DataType.MultilineText)]
		public string Description {
			get; set;
		} = string.Empty;

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Aangemaakt")]
		[DataType(DataType.DateTime)]
		public DateTime Created {
			get; set;
		} = DateTime.Now;

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Verwijderd")]
		[DataType(DataType.DateTime)]
		public DateTime Deleted {
			get; set;
		} = DateTime.MaxValue;

		// Foreign key naar AppointmentType:  Zorg voor de juiste één-op-veel relatie
		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Afspraak type ID")]
		[ForeignKey("AppointmentType")]
		public int AppointmentTypeId {
			get; set;
		} = AppointmentType.Dummy.Id;

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Afspraak type")]
		public AppointmentType AppointmentType {
			get; set;
		} = AppointmentType.Dummy;

		[Required(ErrorMessage = "{0} is vereist")]
		[Display(Name = "Is goedgekeurd")]
		public bool IsApproved {
			get; set;
		} = false;

		public override string ToString() {
			return Id + "  Afspraak op " + From + " betreffende " + Title;
		}

		// Seeding data
		public static List<Appointment> SeedingData() {
			return new() {
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
			};
		}
	}
}