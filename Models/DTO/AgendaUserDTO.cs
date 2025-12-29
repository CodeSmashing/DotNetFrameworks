namespace Models.DTO {
	/// <summary>
	/// Representeert een data transfer object voor een gebruiker in het systeem.
	/// Wordt gebruikt om gegevens tussen de API en de MAUI-client te synchroniseren.
	/// </summary>
	public class AgendaUserDTO {
		/// <summary>
		/// De unieke identificatie van de gebruiker.
		/// </summary>
		public required string Id {
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
