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
		public DateTime Deleted
		{
			get; set;
		} = DateTime.MaxValue;

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

		public static List<ToDo> SeedingData(List<string> appointmentIds) {
			Random rnd = new();
			return new() {
				// Add a dummy ToDo
				Dummy,
				
				// Add a few example ToDos
				new ToDo {
					Description = "Eerste ToDo",
					Ready = false,
					AppointmentId = appointmentIds[rnd.Next(appointmentIds.Count)],
				},
				new ToDo {
					Description = "Tweede ToDo",
					Ready = true,
					AppointmentId = appointmentIds[rnd.Next(appointmentIds.Count)],
				},
				new ToDo {
					Description = "Derde ToDo",
					Ready = false ,
					AppointmentId = appointmentIds[rnd.Next(appointmentIds.Count)],
				},
				new ToDo {
					Description = "Eerste ToDo",
					Ready = false,
					AppointmentId = appointmentIds[rnd.Next(appointmentIds.Count)],
				},
				new ToDo {
					Description = "Tweede ToDo",
					Ready = true,
					AppointmentId = appointmentIds[rnd.Next(appointmentIds.Count)],
				},
				new ToDo {
					Description = "Derde ToDo",
					Ready = false ,
					AppointmentId = appointmentIds[rnd.Next(appointmentIds.Count)],
				}
			};
		}
	}
}
