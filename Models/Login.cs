using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models {
	/// <summary>
	/// Representeert het model voor het inloggen van een <see cref="AgendaUser"/>.
	/// </summary>
	public class Login {
		/// <summary>
		/// Het e-mailadres van de gebruiker dat dient als unieke identificatie.
		/// </summary>
		/// <remarks>
		/// Dit wordt als sleutel gebruikt omdat het <c>Email</c>-veld van het <see cref="AgendaUser"/>-model 
		/// verplicht en uniek is.
		/// </remarks>
		[Key]
		[Display(Name = "Email")]
		public required string Email {
			get; set;
		}

		/// <summary>
		/// Het wachtwoord dat hoort bij het account van de gebruiker.
		/// </summary>
		/// <remarks>
		/// De invoer wordt verborgen door <see cref="PasswordPropertyTextAttribute"/>.
		/// </remarks>
		[Display(Name = "Password")]
		[PasswordPropertyText]
		public required string Password {
			get; set;
		}

		/// <summary>
		/// Geeft aan of de inlogsessie bewaard moet blijven na het sluiten van de browser.
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
