using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models {
	public class AgendaUser : IdentityUser {
		public static AgendaUser Dummy;

		[StringLength(20, MinimumLength = 2)]
		[Display(Name = "FirstName", ResourceType = typeof(Resources.AgendaUser))]
		public required string FirstName {
			get; set;
		}

		[StringLength(20, MinimumLength = 2)]
		[Display(Name = "LastName", ResourceType = typeof(Resources.AgendaUser))]
		public required string LastName {
			get; set;
		}

		[StringLength(20, MinimumLength = 5)]
		[Display(Name = "DisplayName", ResourceType = typeof(Resources.AgendaUser))]
		public string? DisplayName {
			get; set;
		}

		[Display(Name = "LanguageCode", ResourceType = typeof(Resources.AgendaUser))]
		[ForeignKey("Language")]
		public string LanguageCode {
			get; set;
		} = "nl";

		[Display(Name = "Language", ResourceType = typeof(Resources.AgendaUser))]
		public Language? Language {
			get; set;
		}

		[Display(Name = "VehicleId", ResourceType = typeof(Resources.AgendaUser))]
		[ForeignKey("Vehicle")]
		public string? VehicleId {
			get; set;
		}

		[Display(Name = "Vehicle", ResourceType = typeof(Resources.AgendaUser))]
		public Vehicle? Vehicle {
			get; set;
		}

		[Display(Name = "Created")]
		[DataType(DataType.DateTime)]
		public DateTime Created {
			get; private set;
		} = DateTime.Now;

		[Display(Name = "Deleted")]
		[DataType(DataType.DateTime)]
		public DateTime? Deleted {
			get; set;
		}

		public AgendaUser() {
			// Automatically assign a GUID-based UserName as ASP.NET Core doesn't allow empty usernames
			UserName = Id;
		}

		override public string ToString() {
			return string.Format(Resources.AgendaUser.ToString, FirstName, LastName, DisplayName);
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
