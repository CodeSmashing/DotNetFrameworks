using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models {
	public class ToDo {
		static public readonly ToDo Dummy = new() { Title = "-", Ready = false, AppointmentId = 0 };

		public long Id {
			get; set;
		}

		// Eigenschappen
		[Required]
		[Display(Name = "Title")]
		public string Title { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Klaar")]
		public bool Ready { get; set; } = false;

		// Foreign key naar Appointment
		[Required]
		[Display(Name = "Appointment")]
		[ForeignKey("Appointment")]
		public int AppointmentId {
			get; set;
		}

		// Override ToString methode
		public override string ToString() {
			return $"ToDoId: {Id} | Titel: {Title} | Is klaar?({Ready})";
		}

		// Seeding data
		public static List<ToDo> SeedingData() {
			return new() {
				// Voeg een dummy ToDo toe
				Dummy,

				// Voeg enkele voorbeeld ToDos toe
				new ToDo { Title = "Eerste ToDo", Ready = false, AppointmentId = 1},
				new ToDo { Title = "Tweede ToDo", Ready = true, AppointmentId = 1},
				new ToDo { Title = "Derde ToDo", Ready = false , AppointmentId = 1},
				new ToDo { Title = "Eerste ToDo", Ready = false, AppointmentId = 2},
				new ToDo { Title = "Tweede ToDo", Ready = true, AppointmentId = 2},
				new ToDo { Title = "Derde ToDo", Ready = false , AppointmentId = 2 }
			};
		}
	}
}
