using GardenPlanner_Web.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.CustomServices;
using Models.DTO;
using System.Net.Mime;

namespace GardenPlanner_Web.Controllers.Api {
	/// <summary>
	/// Beheert account-gerelateerde acties zoals authenticatie,
	/// autorisatie en gebruikersbeheer.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	[AllowAnonymous]
	[Consumes(MediaTypeNames.Application.Json)]
	[Produces(MediaTypeNames.Application.Json)]
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
		/// <param name="userStore">
		/// Voor het bijhouden van de gebruikers.
		/// </param>
		/// <param name="globalAppSettings">
		/// Bevat de globale instellingen voor de applicatie, zoals geconfigureerd in het
		/// systeem.
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
		/// Een <see cref="ActionResult{AgendaUserDTO}"/> die aangeeft of de gebruiker
		/// geautoriseerd is met of zonder de gebruiker.
		/// </returns>
		[HttpGet("isAuthorized")]
		[ProducesResponseType<AgendaUserDTO>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<AgendaUserDTO>> IsAuthorized() {
			var user = await _userManager.GetUserAsync(User);

			if (user != null) {
				return Ok(user.ToDTO());
			}

			return Unauthorized(new {
				Message = "Sessie is verlopen. Log opnieuw in."
			});
		}

		// POST: api/Accounts/logIn
		/// <summary>
		/// Verwerkt een aanvraag om in te loggen in het systeem.
		/// </summary>
		/// <param name="model">
		/// De inloggegevens van de gebruiker (bijv. gebruikersnaam en wachtwoord).
		/// </param>
		/// <returns>
		/// Een <see cref="ActionResult{AgendaUserDTO}"/> met de gebruiker bij succes.
		/// </returns>
		[HttpPost("logIn")]
		[ProducesResponseType<AgendaUserDTO>(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<AgendaUserDTO>> LogIn([FromBody] Login model) {
			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user == null) {
				return Unauthorized(new {
					Message = "E-mailadres of wachtwoord is onjuist."
				});
			}

			// Dit telt niet mee voor accountblokkering bij het inloggen.
			// Om inlogfouten wel accountblokkering te activeren, stel lockoutOnFailure: true in
			var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

			if (result.Succeeded) {
				// Als aangemeld, stuur de gebruiker terug
				return Ok(user.ToDTO());
			}

			return Unauthorized(new {
				Message = "E-mailadres of wachtwoord is onjuist."
			});
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
		/// De aangemaakte gebruiker als <see cref="ActionResult{AgendaUserDTO}"/>
		/// bij succes; anders een foutmelding of validatieprobleem.
		/// </returns>
		[HttpPost("register")]
		[ProducesResponseType<AgendaUserDTO>(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<AgendaUserDTO>> Register([FromBody] Register model) {
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
					return CreatedAtAction(
						nameof(Register),
						new {
							id = user.Id,
							Message = "Account aangemaakt."
						},
						user.ToDTO());
				}

				// Als bevestiging vereist is, sturen we een 200 met een specifieke status
				return CreatedAtAction(
					nameof(Register),
					new {
						id = user.Id,
						Message = "Account aangemaakt. Bevestig je e-mailadres."
					},
					user.ToDTO());
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