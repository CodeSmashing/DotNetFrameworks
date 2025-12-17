using Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

	/// <summary>
	/// Representeert een afspraak type dat gebruikt word voor locale opslag synchronisatie, met een expliciet gezette identificatiecode.
	/// </summary>
	/// <remarks>
	/// <see cref="LocalAppointmentType"/> breidt <see cref="AppointmentType"/> uit door een <c>Id</c> eigenschap
	/// toe te voegen die niet door de database wordt gegenereerd. Dit is handig in scenario's waarin afspraken
	/// lokaal moeten worden gevolgd of gesynchroniseerd en de identificaties door de applicatie in plaats van
	/// de database worden toegewezen.
	/// </remarks>
	public class LocalAppointmentType : AppointmentType {
		/// <summary>
		/// Hiermee wordt de unieke identificatiecode voor de entiteit opgehaald of ingesteld.
		/// </summary>
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public new string Id {
			get; private set;
		} = Guid.NewGuid().ToString();
	}
}