using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models {
	/// <summary>
	/// Representeert het model voor het registreren van een
	/// <see cref="AgendaUser"/>.
	/// </summary>
	public class Register {
		/// <summary>
		/// Het e-mailadres van de gebruiker dat dient als unieke identificatie.
		/// </summary>
		/// <remarks>
		/// Dit wordt als sleutel gebruikt omdat het <c>Email</c>-veld
		/// van het <see cref="AgendaUser"/>-model 
		/// verplicht en uniek is.
		/// </remarks>
		[Key]
		[Display(Name = "Email")]
		public required string Email {
			get; set;
		}

		/// <summary>
		/// De gewenste weergavenaam voor het account.
		/// </summary>
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		public string? DisplayName {
			get; set;
		}

		/// <summary>
		/// De voornaam van de gebruiker.
		/// </summary>
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
		public required string FirstName {
			get; set;
		}

		/// <summary>
		/// De achternaam van de gebruiker.
		/// </summary>
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
		public required string LastName {
			get; set;
		}

		/// <summary>
		/// Het wachtwoord dat hoort bij het account van de gebruiker.
		/// </summary>
		/// <remarks>
		/// De invoer wordt verborgen door
		/// <see cref="PasswordPropertyTextAttribute"/>.
		/// </remarks>
		[Display(Name = "Password")]
		[PasswordPropertyText]
		public required string Password {
			get; set;
		}

		/// <summary>
		/// Bevestiging van het gekozen wachtwoord.
		/// </summary>
		/// <remarks>
		/// Dit veld moet exact overeenkomen met het <see cref="Password"/>
		/// -veld.
		/// </remarks>
		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public required string ConfirmPassword {
			get; set;
		}

		/// <summary>
		/// Geeft aan of de inlogsessie bewaard moet blijven na het
		/// sluiten van de browser.
		/// </summary>
		[Display(Name = "RememberMe")]
		public bool RememberMe {
			get; set;
		} = false;

		/// <summary>
		/// De datum en tijd waarop de inlogsessie verloopt.
		/// </summary>
		/// <remarks>
		/// Standaard ingesteld op één uur na het huidige tijdstip.
		/// </remarks>
		[Display(Name = "ExpiresAt")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
		public DateTime ExpiresAt {
			get; set;
		} = DateTime.Now + TimeSpan.FromHours(1);
	}
}
