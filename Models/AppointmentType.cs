using Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models {
	public class AppointmentType {
		public static AppointmentType Dummy;

		public string Id {
			get; private set;
		} = Guid.NewGuid().ToString();

		[Display(Name = "AppointmentTypeName", ResourceType = typeof(Resources.AppointmentType))]
		public required AppointmentTypeName Name {
			get; set;
		}

		[StringLength(2000, MinimumLength = 3)]
		[Display(Name = "Description", ResourceType = typeof(Resources.AppointmentType))]
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

		public override string ToString() {
			return string.Format(Resources.AppointmentType.ToString, Id, Name, Description, Created);
		}

		public static AppointmentType[] SeedingData() {
			Dummy = new() {
				Id = "-",
				Created = new DateTime(2000, 1, 1),
				Description = string.Empty,
				Name = AppointmentTypeName.Kennismaking
			};

			return [
				// Add a dummy AppointmentType
				Dummy,
				
				// Add a few example AppointmentTypes
				new() {
					Name = AppointmentTypeName.Onderhoud,
					Description = "Algemeen onderhoud"
				},
				new() {
					Name = AppointmentTypeName.Aanleg,
					Description = "Aanleggen van tuin"
				},
				new() {
					Name = AppointmentTypeName.Kennismaking,
					Description = "Kennismaking klant"
				}
			];
		}
	}
}