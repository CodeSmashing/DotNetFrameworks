using GardenPlanner_Web.Properties;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace GardenPlanner_Web.Controllers {
	public class LanguagesController : Controller {
		private readonly GlobalAppSettings _globalAppSettings;

		public LanguagesController(GlobalAppSettings globalAppSettings) {
			_globalAppSettings = globalAppSettings;
		}

		public IActionResult ChangeLanguage(string code, string returnUrl) {
			Response.Cookies.Append(
				CookieRequestCultureProvider.DefaultCookieName,
				CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(code)),
				new CookieOptions { Expires = _globalAppSettings.DefaultCookieLifespan }
			);

			return LocalRedirect(returnUrl);
		}
	}
}
