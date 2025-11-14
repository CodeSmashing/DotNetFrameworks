using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models {
	public class ToDo {
		static public readonly ToDo Dummy = new() {
			Description = string.Empty,
			AppointmentId = Appointment.Dummy.Id
		};

		public string Id {
			get; private set;
		} = Guid.NewGuid().ToString();

		[Display(Name = "Verwijderd")]
		[DataType(DataType.DateTime)]
		public DateTime? Deleted {
			get; set;
		}

		[Display(Name = "Title")]
		public required string Description {
			get; set;
		}

		[Required]
		[Display(Name = "Klaar")]
		public bool Ready {
			get; set;
		} = false;

		[Display(Name = "Afspraak ID")]
		[ForeignKey("Appointment")]
		public required string AppointmentId {
			get; set;
		}

		[Display(Name = "Afspraak")]
		public Appointment? Appointment {
			get; set;
		}

		public override string ToString() {
			return $"Id: {Id} | Description: {Description} | Is klaar?({Ready})";
		}

		public static ToDo[] SeedingData(string[] appointmentIds) {
			Random rnd = new();
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
}
