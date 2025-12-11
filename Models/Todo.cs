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
}
