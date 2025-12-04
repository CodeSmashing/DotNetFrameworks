using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models {
	public class Appointment {
		public static Appointment Dummy;

		public string Id {
			get; private set;
		} = Guid.NewGuid().ToString();

		[ForeignKey("AgendaUser")]
		[Display(Name = "UserId", ResourceType = typeof(Resources.Appointment))]
		public required string AgendaUserId {
			get; set;
		}

		[Display(Name = "User", ResourceType = typeof(Resources.Appointment))]
		public AgendaUser? AgendaUser {
			get; set;
		}

		[Display(Name = "Date", ResourceType = typeof(Resources.Appointment))]
		[DataType(DataType.DateTime)]
		public required DateTime Date {
			get; set;
		}

		[StringLength(50, MinimumLength = 3)]
		[Display(Name = "Title", ResourceType = typeof(Resources.Appointment))]
		public required string Title {
			get; set;
		}

		[StringLength(2000, MinimumLength = 3)]
		[Display(Name = "Description", ResourceType = typeof(Resources.Appointment))]
		[DataType(DataType.MultilineText)]
		public required string Description {
			get; set;
		}

		[Display(Name = "Created")]
		[DataType(DataType.DateTime)]
		public DateTime Created {
			get; private set;
		} = DateTime.Now;

		[Display(Name = "Deleted")]
		[DataType(DataType.DateTime)]
		public DateTime? Deleted {
			get; set;
		}

		[Display(Name = "AppointmentTypeId", ResourceType = typeof(Resources.Appointment))]
		[ForeignKey("AppointmentType")]
		public required string AppointmentTypeId {
			get; set;
		}

		[Display(Name = "AppointmentType", ResourceType = typeof(Resources.Appointment))]
		public AppointmentType? AppointmentType {
			get; set;
		}

		[Display(Name = "IsApproved", ResourceType = typeof(Resources.Appointment))]
		public bool IsApproved {
			get; set;
		} = false;

		[Display(Name = "IsCompleted", ResourceType = typeof(Resources.Appointment))]
		public bool IsCompleted {
			get; set;
		} = false;

		public override string ToString() {
			return String.Format(Resources.Appointment.ToString, Id, Date, Title);
		}

		public static Appointment[] SeedingData(string[] listUserIds, string[] appointmentTypeIds) {
			Random rnd = new();
			Dummy = new() {
				Id = "-",
				Created = new DateTime(2000, 1, 1),
				Date = new DateTime(2000, 1, 1),
				AgendaUserId = AgendaUser.Dummy.Id,
				Title = string.Empty,
				Description = string.Empty,
				AppointmentTypeId = AppointmentType.Dummy.Id,
				IsCompleted = true
			};

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