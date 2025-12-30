using GardenPlanner_Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GardenPlanner_Web.Controllers {
	/// <summary>
	/// Beheert de interactie en weergave van de thuis pagina in de webinterface.
	/// </summary>
	public class HomeController : Controller {
		private readonly ILogger<HomeController> _logger;

		/// <summary>
		/// Initialiseert een nieuwe instantie van de
		/// <see cref="HomeController"/> klasse.
		/// </summary>
		/// <param name="logger">
		/// De service die gebruikt wordt voor het loggen van fouten, waarschuwingen
		/// en diagnostische informatie.
		/// </param>
		public HomeController(ILogger<HomeController> logger) {
			_logger = logger;
		}

		/// <summary>
		/// Toont de thuis of de afspraken pagina.
		/// </summary>
		/// <returns>
		/// De index view van ofwel de thuis of de afspraken pagina.
		/// </returns>
		[HttpGet]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status302Found)]
		public IActionResult Index() {
			if(User.IsInRole("User")) {
				return Redirect("Appointments");
			}
			return View();
		}

		/// <summary>
		/// Toont de privacy pagina.
		/// </summary>
		/// <returns>
		/// De index view van de privacy pagina.
		/// </returns>
		[HttpGet]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		public IActionResult Privacy() {
			return View();
		}

		/// <summary>
		/// Toont de error pagina.
		/// </summary>
		/// <returns>
		/// De index view van de error pagina.
		/// </returns>
		[HttpGet]
		[ProducesResponseType<ViewResult>(StatusCodes.Status200OK)]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error() {
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
