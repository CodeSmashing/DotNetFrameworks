using GardenPlanner_Web.Properties;
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
		private readonly IUserStore<AgendaUser> _userStore;
		private readonly IUserEmailStore<AgendaUser> _emailStore;
		private readonly GlobalAppSettings _globalAppSettings;

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
			UserManager<AgendaUser> userManager,
			IUserStore<AgendaUser> userStore,
			GlobalAppSettings globalAppSettings) {
			_signInManager = signInManager;
			_userManager = userManager;
			_userStore = userStore;
			_emailStore = GetEmailStore();
			_globalAppSettings = globalAppSettings;
		}

		// GET: api/Accounts/isAuthorized
		/// <summary>
		/// Controleert of de huidige gebruiker geauthenticeerd is en over de
		/// juiste rechten beschikt.
		/// </summary>
		/// <returns>
		/// Een <see cref="ActionResult{AgendaUser}"/> die aangeeft of de gebruiker geautoriseerd is met of zonder de gebruiker.
		/// </returns>
		/// <response code="200">
		/// De gebruiker is geautoriseerd.
		/// </response>
		/// <response code="401">
		/// De gebruiker is niet geauthenticeerd of de sessie is verlopen.
		/// </response>
		[HttpGet]
		[Route("isAuthorized")]
		public async Task<ActionResult<AgendaUser>> IsAuthorized() {
			if (User.Identity?.IsAuthenticated ?? false) {
				// Als aangemeld, stuur de gebruiker terug
				return Ok(await _userManager.FindByNameAsync(User.Identity?.Name!));
			}
			return Unauthorized("E-mailadres of wachtwoord is onjuist.");
		}

		// POST: api/Accounts/logIn
		/// <summary>
		/// Verwerkt een aanvraag om in te loggen in het systeem.
		/// </summary>
		/// <param name="model">
		/// De inloggegevens van de gebruiker (bijv. gebruikersnaam en wachtwoord).
		/// </param>
		/// <returns>
		/// Een <see cref="ActionResult{AgendaUser}"/> met de gebruiker bij succes.
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
		public async Task<ActionResult<AgendaUser>> LogIn(Login model) {
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
				// Als aangemeld, stuur de gebruiker terug
				// Is mischien niet het meest veilige, zou hiervoor een DTO kunnen maken ofzo.
				return Ok(contextUser);
			}
			return Unauthorized("E-mailadres of wachtwoord is onjuist.");
		}

		// POST: api/Accounts/register
		/// <summary>
		/// Registreert een nieuwe gebruiker in het systeem op basis van
		/// de opgegeven gegevens.
		/// </summary>
		/// <param name="model">
		/// Het model met de registratiegegevens van de gebruiker.
		/// </param>
		/// <returns>
		/// De aangemaakte gebruiker als <see cref="ActionResult{AgendaUser}"/>
		/// bij succes; anders een foutmelding of validatieprobleem.
		/// </returns>
		/// <response code="200">
		/// De gebruiker is succesvol aangemaakt en ingelogd.
		/// </response>
		/// <response code="400">
		/// De invoergegevens zijn ongeldig of de registratie is mislukt.
		/// </response>
		[HttpPost]
		[Route("register")]
		public async Task<ActionResult<AgendaUser>> Register(Register model) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			var user = CreateUser();

			// Gegevens instellen
			await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);

			user.DisplayName = model.DisplayName;
			user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.LanguageCode = _globalAppSettings.DefaultLanguageCode;

			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded) {
				// Voeg standaardrol toe
				await _userManager.AddToRoleAsync(user, "USER");

				// Log direct in als er geen verdere bevestiging nodig is
				if (!_userManager.Options.SignIn.RequireConfirmedAccount) {
					await _signInManager.SignInAsync(user, isPersistent: false);

					// Als aangemeld, stuur de gebruiker terug
					// Is mischien niet het meest veilige, zou hiervoor een DTO kunnen maken ofzo.
					return Ok(user);
				}

				// Als bevestiging vereist is, sturen we een 200 met een specifieke status
				return Ok(new {
					Message = "Account aangemaakt. Bevestig je e-mailadres."
				});
			}

			// Voeg errors toe aan de ModelState voor een duidelijk BadRequest antwoord
			foreach (var error in result.Errors) {
				ModelState.AddModelError("Registration", error.Description);
			}

			return BadRequest(ModelState);
		}

		private AgendaUser CreateUser() {
			try {
				return Activator.CreateInstance<AgendaUser>();
			} catch {
				throw new InvalidOperationException($"Can't create an instance of '{nameof(AgendaUser)}'. " +
					 $"Ensure that '{nameof(AgendaUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
					 $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
			}
		}

		private IUserEmailStore<AgendaUser> GetEmailStore() {
			if (!_userManager.SupportsUserEmail) {
				throw new NotSupportedException("The default UI requires a user store with email support.");
			}
			return (IUserEmailStore<AgendaUser>) _userStore;
		}
	}
}