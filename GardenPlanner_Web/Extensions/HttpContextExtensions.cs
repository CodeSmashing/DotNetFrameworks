namespace GardenPlanner_Web.Extensions {
	/// <summary>
	/// Bevat uitbreiding-methoden voor <see cref="HttpContext"/>
	/// om gegevensextractie te vereenvoudigen.
	/// </summary>
	public static class HttpContextExtensions {
		private const string UserIdKey = "UserId";

		/// <summary>
		/// Haalt de unieke identificatie van de gebruiker
		/// op uit de huidige aanvraagcontext.
		/// </summary>
		/// <remarks>
		/// Hiervoor is toegang tot de actieve verzoek vereist.<br/>
		/// Deze waarde wordt doorgaans geplaatst door de
		/// authenticatie-middleware in de
		/// <see cref="HttpContext.Items"/> collectie.
		/// </remarks>
		/// <param name="context">
		/// De huidige <see cref="HttpContext"/> van de aanvraag.
		/// </param>
		/// <returns>
		/// De gebruikers-ID als een <see cref="string"/>
		/// indien aanwezig; anders <c>null</c>
		/// </returns>
		public static string? GetUserId(this HttpContext context) {
			return (string?) context.Items[UserIdKey];
		}
	}
}
