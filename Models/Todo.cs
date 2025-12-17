using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models {
	public class ToDo {
		public static ToDo Dummy;

		public string Id {
			get; private set;
		} = Guid.NewGuid().ToString();

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

		[Display(Name = "Description")]
		public required string Description {
			get; set;
		}

		[Required]
		[Display(Name = "Ready")]
		public bool Ready {
			get; set;
		} = false;

		[Display(Name = "AppointmentId")]
		[ForeignKey("Appointment")]
		public required string AppointmentId {
			get; set;
		}

		[Display(Name = "Appointment")]
		public Appointment? Appointment {
			get; set;
		}

		public override string ToString() {
			return string.Format(Resources.ToDo.ToString, Id, Description, Ready);
		}

		public static ToDo[] SeedingData(string[] appointmentIds) {
			Random rnd = new();
			Dummy = new() {
				Id = "-",
				Created = new DateTime(2000, 1, 1),
				Description = string.Empty,
				AppointmentId = Appointment.Dummy.Id
			};

			return [
				// Add a dummy ToDo
				Dummy,
				
				// Add a few example ToDos
				new() {
					Description = "Eerste ToDo",
					Ready = false,
					AppointmentId = appointmentIds[rnd.Next(appointmentIds.Length)],
				},
				new() {
					Description = "Tweede ToDo",
					Ready = true,
					AppointmentId = appointmentIds[rnd.Next(appointmentIds.Length)],
				},
				new() {
					Description = "Derde ToDo",
					Ready = false ,
					AppointmentId = appointmentIds[rnd.Next(appointmentIds.Length)],
				},
				new() {
					Description = "Eerste ToDo",
					Ready = false,
					AppointmentId = appointmentIds[rnd.Next(appointmentIds.Length)],
				},
				new() {
					Description = "Tweede ToDo",
					Ready = true,
					AppointmentId = appointmentIds[rnd.Next(appointmentIds.Length)],
				},
				new() {
					Description = "Derde ToDo",
					Ready = false ,
					AppointmentId = appointmentIds[rnd.Next(appointmentIds.Length)],
				}
			];
		}
	}

	/// <summary>
	/// Representeert een to-do dat gebruikt word voor locale opslag synchronisatie, met een expliciet gezette identificatiecode, afspraak identificatiecode, en afspraak referentie eigenschap.
	/// </summary>
	/// <remarks>
	/// <see cref="LocalToDo"/> breidt <see cref="ToDo"/> uit door een <c>Id</c> eigenschap
	/// toe te voegen die niet door de database wordt gegenereerd. Dit is handig in scenario's waarin voertuigen
	/// lokaal moeten worden gevolgd of gesynchroniseerd en de identificaties door de applicatie in plaats van
	/// de database worden toegewezen.
	/// </remarks>
	public class LocalToDo : ToDo {
		/// <summary>
		/// Hiermee wordt de unieke identificatiecode voor de entiteit opgehaald of ingesteld.
		/// </summary>
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public new string Id {
			get; private set;
		} = Guid.NewGuid().ToString();

		/// <summary>
		/// Hiermee wordt de lokale identificatiecode voor het bijbehorende to-do opgehaald of ingesteld, die wordt gebruikt bij lokale (SQLite) synchronisatie.
		/// </summary>
		public string LocalAppointmentId {
			get => base.AppointmentId;
			set => base.AppointmentId = value;
		}

		/// <summary>
		/// Hiermee wordt de lokale afspraak dat aan deze to-do is gekoppeld, opgehaald of ingesteld.
		/// </summary>
		/// <remarks>
		/// Deze eigenschap overschrijft de basiseigenschap <c>Appointment</c> om
		/// lokale (SQLite) synchronisatiescenario's te ondersteunen met behulp van <see cref="LocalAppointment"/>.
		/// </remarks>
		public new LocalAppointment? Appointment {
			get; set;
		}
	}
}
