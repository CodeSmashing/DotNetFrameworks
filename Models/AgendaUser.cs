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
		} = null!;

		[Required(ErrorMessage = "{0} is vereist")]
		[StringLength(20, MinimumLength = 2, ErrorMessage = "De achternaam moet minstens 2 characters en mag maximum 20 characters bevatten")]
		[Display(Name = "Achternaam")]
		public string LastName {
			get; set;
		} = null!;

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

		[Required(ErrorMessage = "{0} is vereist")]
		[EmailAddress]
		[Display(Name = "Email")]
		public override string? Email {
			get => base.Email;
			set => base.Email = value;
		}

		[Display(Name = "Aangemaakt")]
		[DataType(DataType.DateTime)]
		public DateTime Created {
			get; private set;
		} = DateTime.Now;

		[Display(Name = "Verwijderd")]
		[DataType(DataType.DateTime)]
		public DateTime Deleted {
			get; set;
		} = DateTime.MaxValue;

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
			};
		}
	}
}
