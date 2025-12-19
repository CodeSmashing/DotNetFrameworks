using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace GardenPlanner_Web.Controllers.Api {
	/// <summary>
	/// Beheert account-gerelateerde acties zoals authenticatie,
	/// autorisatie en gebruikersbeheer.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class AccountsController : ControllerBase {
		private readonly SignInManager<AgendaUser> _signInManager;
		private readonly UserManager<AgendaUser> _userManager;

		/// <summary>
		/// Initialiseert een nieuwe instantie van de <see cref="AccountsController"/> klasse.
		/// </summary>
		/// <param name="signInManager">
		/// De manager die verantwoordelijk is voor het afhandelen van
		/// het inlogproces en de gebruikerssessies.
		/// </param>
		/// <param name="userManager">
		/// De manager die verantwoordelijk is voor het beheren van
		/// gebruikersgegevens, rollen en wachtwoorden.
		/// </param>
		public AccountsController(
			SignInManager<AgendaUser> signInManager,
			UserManager<AgendaUser> userManager) {
			_signInManager = signInManager;
			_userManager = userManager;
		}

		// GET: api/Accounts/isAuthorized
		/// <summary>
		/// Controleert of de huidige gebruiker geauthenticeerd is en over de
		/// juiste rechten beschikt.
		/// </summary>
		/// <returns>
		/// Een <see cref="ActionResult"/> die aangeeft of de gebruiker geautoriseerd is met of zonder de gebruiker.
		/// </returns>
		/// <response code="200">
		/// De gebruiker is geautoriseerd.
		/// </response>
		/// <response code="401">
		/// De gebruiker is niet geauthenticeerd of de sessie is verlopen.
		/// </response>
		[HttpGet]
		[Route("isAuthorized")]
		public async Task<ActionResult> IsAuthorized() {
			if (User.Identity?.IsAuthenticated ?? false) {
				// Als aangemeld, stuur de gebruiker terug
				return Ok(await _userManager.FindByNameAsync(User.Identity?.Name!));
			}
			return Unauthorized("E-mailadres of wachtwoord is onjuist.");
		}

		// POST: api/Accounts/logIn
		// <summary>
		// Verwerkt een aanvraag om in te loggen in het systeem.
		// </summary>
		// <param name="model">
		// De inloggegevens van de gebruiker (bijv. gebruikersnaam en wachtwoord).
		// </param>
		// <returns>
		// Een <see cref="IActionResult"/> met een toegangstoken (bijv. JWT) bij succes.
		// </returns>
		// <response code="200">
		// Inloggen succesvol; retourneert het authenticatietoken.
		// </response>
		// <response code="400">
		// Ongeldige aanvraaggegevens opgegeven.
		// </response>
		// <response code="401">
		// Ongeldige gebruikersnaam of wachtwoord.
		// </response>
		/// <summary>
		/// Verwerkt een aanvraag om in te loggen in het systeem.
		/// </summary>
		/// <param name="model">
		/// De inloggegevens van de gebruiker (bijv. gebruikersnaam en wachtwoord).
		/// </param>
		/// <returns>
		/// Een <see cref="IActionResult"/> met de gebruiker bij succes.
		/// </returns>
		/// <response code="200">
		/// Inloggen succesvol; retourneert de gebruiker.
		/// </response>
		/// <response code="400">
		/// Ongeldige aanvraaggegevens opgegeven.
		/// </response>
		/// <response code="401">
		/// Ongeldige gebruikersnaam of wachtwoord.
		/// </response>
		[HttpPost]
		[Route("logIn")]
		public async Task<IActionResult> LogIn(Login model) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			AgendaUser? contextUser = await _userManager.FindByEmailAsync(model.Email);
			if (contextUser == null) {
				return Unauthorized("E-mailadres of wachtwoord is onjuist.");
			}

			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, set lockoutOnFailure: true
			var result = await _signInManager.PasswordSignInAsync(contextUser.UserName!, model.Password, model.RememberMe, lockoutOnFailure: false);

			if (result.Succeeded) {
				if (User.Identity?.IsAuthenticated ?? false) {
					// Als aangemeld, stuur de gebruiker terug
					// Is mischien niet het meest veilige, zou hiervoor een DTO kunnen maken ofzo.
					return Ok(contextUser);
				}
			}
			return Unauthorized("E-mailadres of wachtwoord is onjuist.");
		}
	}
}