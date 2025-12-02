using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models {
	public class AgendaUser : IdentityUser {
		public static AgendaUser Dummy;

		[StringLength(20, MinimumLength = 2, ErrorMessage = "De voornaam moet minstens 2 characters en mag maximum 20 characters bevatten")]
		[Display(Name = "Voornaam")]
		public required string FirstName {
			get; set;
		}

		[StringLength(20, MinimumLength = 2, ErrorMessage = "De achternaam moet minstens 2 characters en mag maximum 20 characters bevatten")]
		[Display(Name = "Achternaam")]
		public required string LastName {
			get; set;
		}

		[StringLength(20, MinimumLength = 5, ErrorMessage = "De gebruikersnaam moet minstens 5 characters en mag maximum 20 characters bevatten")]
		[Display(Name = "Gebruikersnaam")]
		public string? DisplayName {
			get; set;
		}

		[Display(Name = "Voorkeur taal code")]
		[ForeignKey("Language")]
		public string LanguageCode {
			get; set;
		} = "nl";

		[Display(Name = "Voorkeur taal")]
		public Language? Language {
			get; set;
		}

		[Display(Name = "Voertuig ID")]
		[ForeignKey("Vehicle")]
		public string? VehicleId {
			get; set;
		}

		[Display(Name = "Voertuig")]
		public Vehicle? Vehicle {
			get; set;
		}

		[Display(Name = "Aangemaakt")]
		[DataType(DataType.DateTime)]
		public DateTime Created {
			get; private set;
		} = DateTime.Now;

		[Display(Name = "Verwijderd")]
		[DataType(DataType.DateTime)]
		public DateTime? Deleted {
			get; set;
		}

		public AgendaUser() {
			// Automatically assign a GUID-based UserName as ASP.NET Core doesn't allow empty usernames
			UserName = Id;
		}

		override public string ToString() {
			return $"{FirstName} {LastName} ({DisplayName})";
		}

		public static AgendaUser[] SeedingData() {
			Dummy = new() {
				Id = "-",
				Created = new DateTime(2000, 1, 1),
				FirstName = "-",
				LastName = "-",
				DisplayName = "dummy",
				Email = "dummy@gardenDb.org",
				EmailConfirmed = true,
				LanguageCode = Language.Dummy.Code
			};

			return [
				// Add a dummy AgendaUser
				Dummy,
				
				// Add a few example AgendaUsers
				new() {
					FirstName = "Bob",
					LastName = "Dylan",
					DisplayName = "BobbySmurda",
					Email = "admin.bob@gardenDb.org",
					EmailConfirmed = true
				},
				new() {
					FirstName = "Bart",
					LastName = "De Smet",
					DisplayName = "Bartje",
					Email = "employee.bart@gardenDb.org",
					EmailConfirmed = true
				},
				new() {
					FirstName = "Jeff",
					LastName = "Janssens",
					DisplayName = "Jefke",
					Email = "employee.jeff@gardenDb.org",
					EmailConfirmed = true
				},
				new() {
					FirstName = "Dirk",
					LastName = "De Bakker",
					DisplayName = "Dirkske",
					Email = "employee.dirk@gardenDb.org",
					EmailConfirmed = true
				},
				new() {
					FirstName = "Wim",
					LastName = "De Molenaar",
					DisplayName = "Wim",
					Email = "useradmin.wim@gardenDb.org",
					EmailConfirmed = true
				},
				new() {
					FirstName = "Jeff",
					LastName = "Broek",
					DisplayName = "Alexander",
					Email = "jeff.broek@gmail.com",
					EmailConfirmed = true
				},
				new() {
					FirstName = "Emanuel",
					LastName = "Ross",
					Email = "rosse.emanuel@hotmail.com",
					EmailConfirmed = true
				},
				new() {
					FirstName = "Bart",
					LastName = "Somers",
					Email = "bartbartbart@gmail.com",
					EmailConfirmed = true
				}
			];
		}
	}
}
