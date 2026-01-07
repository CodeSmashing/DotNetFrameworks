using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DTO {
	/// <summary>
	/// Representeert een data transfer object voor een afspraak in het systeem.
	/// Wordt gebruikt om gegevens tussen de API en de MAUI-client te synchroniseren.
	/// </summary>
	public class AppointmentDTO {
		/// <summary>
		/// De unieke identificatie van de lokale afpsraak.
		/// </summary>
		public string Id {
			get; private set;
		} = Guid.NewGuid().ToString();

		/// <summary>
		/// De unieke identificatie van de globale afpsraak.
		/// </summary>
		public string? GlobalId {
			get; set;
		}

		/// <summary>
		/// De identificatie van de geassocieerde gebruiker
		/// </summary>
		public required string AgendaUserId {
			get; set;
		}

		/// <summary>
		/// De datum van de afspraak
		/// </summary>
		public required DateTime Date {
			get; set;
		}

		/// <summary>
		/// De titel van de afspraak
		/// </summary>
		public required string Title {
			get; set;
		}

		/// <summary>
		/// De omschrijving van de afspraak
		/// </summary>
		public required string Description {
			get; set;
		}

		/// <summary>
		/// De identificatie van de geassocieerde afspraak type
		/// </summary>
		public required string AppointmentTypeId {
			get; set;
		}
	}
}