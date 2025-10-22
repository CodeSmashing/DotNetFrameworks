using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models {
	public class AgendaUser : IdentityUser {
		public static readonly AgendaUser Dummy = new() {
			Id = "-",
			FirstName = "-",
			LastName = "-",
			UserName = "dummy",
			NormalizedUserName = "DUMMY",
			Email = "dummy@gardenDb.org",
			EmailConfirmed = true
		};

		[Required(ErrorMessage = "{0} is vereist")]
		[StringLength(20, MinimumLength = 2, ErrorMessage = "De voornaam moet minstens 2 characters en mag maximum 50 characters bevatten")]
		[Display(Name = "Voornaam")]
		public string FirstName {
			get; set;
		} = "-";

		[Required(ErrorMessage = "{0} is vereist")]
		[StringLength(20, MinimumLength = 2, ErrorMessage = "De achternaam moet minstens 2 characters en mag maximum 50 characters bevatten")]
		[Display(Name = "Achternaam")]
		public string LastName {
			get; set;
		} = "-";

		[Display(Name = "Voertuig ID")]
		[ForeignKey("Vehicle")]
		public int? VehicleId {
			get; set;
		}

		override public string ToString() {
			return $"{FirstName} {LastName} ({UserName})";
		}

		// Seeding data
		public static List<AgendaUser> SeedingData() {
			return new() {
				new() {
					FirstName = "-",
					LastName = "-",
					UserName = "admin",
					NormalizedUserName = "ADMIN",
					Email = "admin@gardenDb.org",
					EmailConfirmed = true
				},
				new() {
					FirstName = "Bart",
					LastName = "De Smet",
					UserName = "Bartje",
					NormalizedUserName = "EMPLOYEE",
					Email = "employee@gardenDb.org",
					EmailConfirmed = true
				},
				new() {
					FirstName = "Jeff",
					LastName = "Janssens",
					UserName = "Jefke",
					NormalizedUserName = "EMPLOYEE",
					Email = "employee@gardenDb.org",
					EmailConfirmed = true
				},
				new() {
					FirstName = "Dirk",
					LastName = "De Bakker",
					UserName = "Dirkske",
					NormalizedUserName = "EMPLOYEE",
					Email = "employee@gardenDb.org",
					EmailConfirmed = true
				},
				new() {
					FirstName = "-",
					LastName = "-",
					UserName = "useradmin",
					NormalizedUserName = "USERADMIN",
					Email = "useradmin@gardenDb.org",
					EmailConfirmed = true
				},
				new() {
					FirstName = "-",
					LastName = "-",
					UserName = "user",
					NormalizedUserName = "USER",
					Email = "user@gardenDb.org",
					EmailConfirmed = true
				}
			};
		}
	}
}
