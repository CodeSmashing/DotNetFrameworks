using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models {
	public class AgendaUser : IdentityUser {
		public string FirstName { get; set; } = "-";

		public string LastName { get; set; } = "-";

		public static readonly AgendaUser Dummy = new() {
			Id = "-",
			FirstName = "-",
			LastName = "-",
			UserName = "dummy",
			NormalizedUserName = "DUMMY",
			Email = "dummy@gardenDb.org",
			EmailConfirmed = true
		};

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
					FirstName = "-",
					LastName = "-",
					UserName = "employee",
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
