using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DTO {
	/// <summary>
	/// Representeert een data transfer object voor een gebruiker in het systeem.
	/// Wordt gebruikt om gegevens tussen de API en de MAUI-client te synchroniseren.
	/// </summary>
	public class AgendaUserDTO {
		/// <summary>
		/// De unieke identificatie van de lokale gebruiker.
		/// </summary>
		public string Id {
			get; private set;
		} = Guid.NewGuid().ToString();

		/// <summary>
		/// De unieke identificatie van de globale gebruiker.
		/// </summary>
		public string? GlobalId {
			get; set;
		}

		/// <summary>
		/// De voornaam van de gebruiker.
		/// </summary>
		public required string FirstName {
			get; set;
		}

		/// <summary>
		/// De achternaam van de gebruiker.
		/// </summary>
		public required string LastName {
			get; set;
		}
	}
}
