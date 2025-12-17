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
		[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
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
		[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
		public DateTime Created {
			get; private set;
		} = DateTime.Now;

		[Display(Name = "Deleted")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
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
			return string.Format(Resources.Appointment.ToString, Id, Date, Title);
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

	/// <summary>
	/// Representeert een afspraak dat gebruikt word voor locale opslag synchronisatie, met een expliciet gezette identificatiecode, afspraak type identificatiecode, en afspraak type referentie eigenschap.
	/// </summary>
	/// <remarks><see cref="LocalAppointment"/> breidt <see cref="Appointment"/> uit door een <c>Id</c> eigenschap
	/// toe te voegen die niet door de database wordt gegenereerd. Dit is handig in scenario's waarin afspraken
	/// lokaal moeten worden gevolgd of gesynchroniseerd en de identificaties door de applicatie in plaats van
	/// de database worden toegewezen.</remarks>
	public class LocalAppointment : Appointment {
		/// <summary>
		/// Hiermee wordt de unieke identificatiecode voor de entiteit opgehaald of ingesteld.
		/// </summary>
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public new required string Id {
			get; set;
		}

		/// <summary>
		/// Hiermee wordt de lokale identificatiecode voor het bijbehorende afspraak type opgehaald of ingesteld, die wordt gebruikt bij lokale (SQLite) synchronisatie.
		/// </summary>
		public string LocalAppointmentTypeId {
			get => base.AppointmentTypeId;
			set => base.AppointmentTypeId = value;
		}
		
		/// <summary>
		/// Hiermee wordt de lokale afspraak type dat aan deze afspraak is gekoppeld, opgehaald of ingesteld.
		/// </summary>
		/// <remarks>Deze eigenschap overschrijft de basiseigenschap <c>AppointmentType</c> om
		/// lokale (SQLite) synchronisatiescenario's te ondersteunen met behulp van <see cref="LocalAppointmentType"/>.</remarks>
		public new LocalAppointmentType? AppointmentType {
			get; set;
		}
	}
}