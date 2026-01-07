using Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DTO {
	/// <summary>
	/// Representeert een data transfer object voor een afspraak type in het systeem.
	/// Wordt gebruikt om gegevens tussen de API en de MAUI-client te synchroniseren.
	/// </summary>
	public class AppointmentTypeDTO {
		/// <summary>
		/// De unieke identificatie van het lokale afspraak type.
		/// </summary>
		public string Id {
			get; private set;
		} = Guid.NewGuid().ToString();

		/// <summary>
		/// De unieke identificatie van het globale afspraak type.
		/// </summary>
		public string? GlobalId {
			get; set;
		}

		/// <summary>
		/// De naam van het afspraak type
		/// </summary>
		public required AppointmentTypeName Name {
			get; set;
		}

		/// <summary>
		/// De beschrijving van het afspraak type
		/// </summary>
		public required string Description {
			get; set;
		}
	}
}