using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models {
	public class ToDo {
		static public readonly ToDo Dummy = new() { Description = "-", Ready = false, AppointmentId = 0 };

		public long Id {
			get; set;
		}

		[Display(Name = "Verwijderd")]
		[DataType(DataType.DateTime)]
		public DateTime Deleted
		{
			get; set;
		} = DateTime.MaxValue;

		// Eigenschappen
		[Required]
		[Display(Name = "Title")]
		public string Description { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Klaar")]
		public bool Ready { get; set; } = false;

		// Foreign key naar Appointment
		[Required]
		[Display(Name = "Appointment")]
		[ForeignKey("Appointment")]
		public long AppointmentId {
			get; set;
		}

		// Override ToString methode
		public override string ToString() {
			return $"Id: {Id} | Description: {Description} | Is klaar?({Ready})";
		}

		// Seeding data
		public static List<ToDo> SeedingData() {
			return new() {
				// Voeg een dummy ToDo toe
				Dummy,

				// Voeg enkele voorbeeld ToDos toe
				new ToDo { Description = "Eerste ToDo", Ready = false, AppointmentId = 1},
				new ToDo { Description = "Tweede ToDo", Ready = true, AppointmentId = 1},
				new ToDo { Description = "Derde ToDo", Ready = false , AppointmentId = 1},
				new ToDo { Description = "Eerste ToDo", Ready = false, AppointmentId = 2},
				new ToDo { Description = "Tweede ToDo", Ready = true, AppointmentId = 2},
				new ToDo { Description = "Derde ToDo", Ready = false , AppointmentId = 2 }
			};
		}
	}
}
