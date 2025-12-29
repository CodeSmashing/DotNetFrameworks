namespace Models.DTO {
	/// <summary>
	/// Representeert een data transfer object voor een to-do in het systeem.
	/// Wordt gebruikt om gegevens tussen de API en de MAUI-client te synchroniseren.
	/// </summary>
	public class ToDoDTO {
		/// <summary>
		/// De unieke identificatie van de to-do.
		/// </summary>
		public required string Id {
			get; set;
		}

		/// <summary>
		/// De beschrijving van de to-do
		/// </summary>
		public required string Description {
			get; set;
		}

		/// <summary>
		/// De identificatie van de geassocieerde afspraak
		/// </summary>
		public required string AppointmentId {
			get; set;
		}
	}
}