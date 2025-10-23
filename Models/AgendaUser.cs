using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models {
	public class AgendaUser : IdentityUser {
		public static readonly AgendaUser Dummy = new() {
			Id = "-",
			FirstName = "-",
			LastName = "-",
			DisplayName = "dummy",
			Email = "dummy@gardenDb.org",
			EmailConfirmed = true
		};

		[Required(ErrorMessage = "{0} is vereist")]
		[StringLength(20, MinimumLength = 2, ErrorMessage = "De voornaam moet minstens 2 characters en mag maximum 20 characters bevatten")]
		[Display(Name = "Voornaam")]
		public string FirstName {
			get; set;
		} = "-";

		[Required(ErrorMessage = "{0} is vereist")]
		[StringLength(20, MinimumLength = 2, ErrorMessage = "De achternaam moet minstens 2 characters en mag maximum 20 characters bevatten")]
		[Display(Name = "Achternaam")]
		public string LastName {
			get; set;
		} = "-";

		[StringLength(20, MinimumLength = 5, ErrorMessage = "De gebruikersnaam moet minstens 5 characters en mag maximum 20 characters bevatten")]
		[Display(Name = "Gebruikersnaam")]
		public string? DisplayName {
			get; set;
		}

		[Display(Name = "Voertuig ID")]
		[ForeignKey("Vehicle")]
		public int? VehicleId {
			get; set;
		}

		public AgendaUser() {
			// Automatically assign a GUID-based UserName as ASP.NET Core doesn't allow empty usernames
			UserName = Id;
		}

		override public string ToString() {
			return $"{FirstName} {LastName} ({DisplayName})";
		}

		// Seeding data
		public static List<AgendaUser> SeedingData() {
			return new() {
				new() {
					FirstName = "Bob",
					LastName = "Dylan",
					DisplayName = "BobbySmurda",
					Email = "admin@gardenDb.org",
					EmailConfirmed = true
				},
				new() {
					FirstName = "Bart",
					LastName = "De Smet",
					DisplayName = "Bartje",
					Email = "employee@gardenDb.org",
					EmailConfirmed = true
				},
				new() {
					FirstName = "Jeff",
					LastName = "Janssens",
					DisplayName = "Jefke",
					Email = "employee@gardenDb.org",
					EmailConfirmed = true
				},
				new() {
					FirstName = "Dirk",
					LastName = "De Bakker",
					DisplayName = "Dirkske",
					Email = "employee@gardenDb.org",
					EmailConfirmed = true
				},
				new() {
					FirstName = "Wim",
					LastName = "De Molenaar",
					DisplayName = "Wim",
					Email = "useradmin@gardenDb.org",
					EmailConfirmed = true
				},
				new() {
					FirstName = "Jeff",
					LastName = "Broek",
					DisplayName = "Alexander",
					Email = "user@gardenDb.org",
					EmailConfirmed = true
				}
			};
		}
	}
}
