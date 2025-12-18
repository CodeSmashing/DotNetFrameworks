namespace GardenPlanner_Web.Properties {
	/// <summary>
	/// Bevat de globale instellingen voor de applicatie, zoals geconfigureerd in het systeem.
	/// </summary>
	public class GlobalAppSettings {
		/// <summary>
		/// Haalt de standaard levensduur van cookies op of stelt deze in.
		/// </summary>
		public DateTime DefaultCookieLifespan {
			get; set;
		} = default!;

		/// <summary>
		/// Haalt de API-sleutel op of stelt deze in.
		/// </summary>
		public string ApiKey {
			get; set;
		} = default!;

		/// <summary>
		/// Haalt de standaard taalcode op of stelt deze in (bijv. "nl", "nl-NL", "en", of "en-US") voor de applicatie-interface.
		/// </summary>
		public string DefaultLanguageCode {
			get; set;
		} = default!;

		/// <summary>
		/// Haalt de naam van de huidige runtime-omgeving op of stelt deze in (bijv. "Development", "Staging" of "Production").
		/// </summary>
		public string EnvironmentName {
			get; set;
		} = default!;
	}
}
