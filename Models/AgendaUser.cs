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

		private static readonly AgendaUser Dummy = new() {
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

		public static async Task Seeder(AgendaDbContext context) {
			// Add the necessary roles here
			if (!context.Roles.Any()) {
				context.Roles.AddRange(new List<IdentityRole> {
					new() {
						Id = "Admin",
						Name = "Admin",
						NormalizedName = "ADMIN" },
					new() {
						Id = "Employee",
						Name = "Employee",
						NormalizedName = "EMPLOYEE" },
					new() {
						Id = "Guest",
						Name = "Guest",
						NormalizedName = "GUEST" }
				});
				context.SaveChanges();
			}

			if (!context.Users.Any()) {
				context.Add(Dummy);
				context.SaveChanges();

				AgendaUser admin = new() {
					FirstName = "-",
					LastName = "-",
					UserName = "admin",
					NormalizedUserName = "ADMIN",
					Email = "admin@gardenDb.org",
					EmailConfirmed = true
				};
				AgendaUser employee = new() {
					FirstName = "-",
					LastName = "-",
					UserName = "employee",
					NormalizedUserName = "EMPLOYEE",
					Email = "employee@gardenDb.org",
					EmailConfirmed = true
				};
				AgendaUser guest = new() {
					FirstName = "-",
					LastName = "-",
					UserName = "guest",
					NormalizedUserName = "GUEST",
					Email = "guest@gardenDb.org",
					EmailConfirmed = true
				};

				UserManager<AgendaUser> userManager = new(
					new UserStore<AgendaUser>(context),
					null,
					new PasswordHasher<AgendaUser>(),
					null, null, null, null, null, null
				);

				await userManager.CreateAsync(admin, "P@ssword1");
				await userManager.CreateAsync(employee, "P@ssword1");
				await userManager.CreateAsync(guest, "P@ssword1");

				while (context.Users.Count() < 5) {
					// Wait until the users are created
					await Task.Delay(100);
				}

				await userManager.AddToRoleAsync(admin, "Admin");
				await userManager.AddToRoleAsync(employee, "Employee");
				await userManager.AddToRoleAsync(guest, "Guest");
				return;
			}
		}
	}
}
