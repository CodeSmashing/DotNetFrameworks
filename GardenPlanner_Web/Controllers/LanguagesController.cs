using GardenPlanner_Web.Properties;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace GardenPlanner_Web.Controllers {
	/// <summary>
	/// Beheert de interactie en weergave van talen in de webinterface.
	/// </summary>
	public class LanguagesController : Controller {
		private readonly GlobalAppSettings _globalAppSettings;

		/// <summary>
		/// Initialiseert een nieuwe instantie van de
		/// <see cref="LanguagesController"/> klasse.
		/// </summary>
		/// <param name="globalAppSettings">
		/// Bevat de globale instellingen voor de applicatie, zoals geconfigureerd in het
		/// systeem.
		/// </param>
		public LanguagesController(GlobalAppSettings globalAppSettings) {
			_globalAppSettings = globalAppSettings;
		}

		/// <summary>
		/// Wijzigt de taal van de applicatie voor de huidige gebruiker.
		/// </summary>
		/// <param name="code">
		/// De ISO-taalcode (bijv. "nl-BE" of "en-US") waarnaar overgeschakeld moet worden.
		/// </param>
		/// <param name="returnUrl">
		/// De URL van de pagina waar de gebruiker vandaan kwam, om na de wijziging naar terug
		/// te keren.
		/// </param>
		/// <returns>
		/// Een redirect naar de opgegeven <paramref name="returnUrl"/>, 
		/// of naar de startpagina als de return URL ongeldig of lokaal niet vindbaar is.
		/// </returns>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status302Found)]
		public IActionResult ChangeLanguage(string code, string returnUrl) {
			Response.Cookies.Append(
				CookieRequestCultureProvider.DefaultCookieName,
				CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(code)),
				new CookieOptions { Expires = _globalAppSettings.DefaultCookieLifespan }
			);

			if (Url.IsLocalUrl(returnUrl)) {
				return LocalRedirect(returnUrl);
			}

			return RedirectToAction("Index", "Home");
		}
	}
}
