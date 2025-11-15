using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models {
	public class Appointment {
		static public readonly Appointment Dummy = new() {
			Created = new DateTime(2000, 1, 1),
			Date = new DateTime(2000, 1, 1),
			AgendaUserId = AgendaUser.Dummy.Id,
			Title = string.Empty,
			Description = string.Empty,
			AppointmentTypeId = AppointmentType.Dummy.Id
		};

		public string Id {
			get; private set;
		} = Guid.NewGuid().ToString();


		[ForeignKey("AgendaUser")]
		[Display(Name = "Gebruiker ID")]
		public required string AgendaUserId {
			get; set;
		}

		[Display(Name = "Gebruiker")]
		public AgendaUser? AgendaUser {
			get; set;
		}

		[Display(Name = "Datum")]
		[DataType(DataType.DateTime)]
		public required DateTime Date {
			get; set;
		}

		[StringLength(50, MinimumLength = 3, ErrorMessage = "De titel moet minstens 3 characters en mag maximum 50 characters bevatten")]
		[Display(Name = "Titel")]
		public required string Title {
			get; set;
		}

		[StringLength(2000, MinimumLength = 3, ErrorMessage = "De omschrijving moet minstens 3 characters en mag maximum 2000 characters bevatten")]
		[Display(Name = "Omschrijving")]
		[DataType(DataType.MultilineText)]
		public required string Description {
			get; set;
		}

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

		[Display(Name = "Afspraak type ID")]
		[ForeignKey("AppointmentType")]
		public required string AppointmentTypeId {
			get; set;
		}

		[Display(Name = "Afspraak type")]
		public AppointmentType? AppointmentType {
			get; set;
		}

		[Display(Name = "Is goedgekeurd")]
		public bool IsApproved {
			get; set;
		} = false;

		[Display(Name = "Is afgerond")]
		public bool IsCompleted {
			get; set;
		} = false;

		public override string ToString() {
			return Id + "  Afspraak op " + Date + " betreffende " + Title;
		}

		public static Appointment[] SeedingData(string[] listUserIds, string[] appointmentTypeIds) {
			Random rnd = new();
			return [
				// Add a dummy Appointment
				Dummy,
				
				// Add a few example Appointments
				new() {
					AgendaUserId = listUserIds[rnd.Next(listUserIds.Length)],
					Title = "Afspraak met Jan",
					Description = "Bespreking van het tuin ontwerp",
					Date = DateTime.Now.AddDays(2).AddHours(11),
					AppointmentTypeId = appointmentTypeIds[rnd.Next(appointmentTypeIds.Length)],
				},
				new() {
					AgendaUserId = listUserIds[rnd.Next(listUserIds.Length)],
					Title = "Onderhoud tuin bij Piet",
					Description = "Jaarlijks onderhoud van de tuin",
					Date = DateTime.Now.AddDays(5).AddHours(10),
					AppointmentTypeId = appointmentTypeIds[rnd.Next(appointmentTypeIds.Length)],
				},
				new() {
					AgendaUserId = listUserIds[rnd.Next(listUserIds.Length)],
					Title = "Kennismaking met Klaas",
					Description = "Eerste gesprek over mogelijke tuin projecten",
					Date = DateTime.Now.AddDays(7).AddHours(15),
					AppointmentTypeId = appointmentTypeIds[rnd.Next(appointmentTypeIds.Length)],
				}
			];
		}
	}
}