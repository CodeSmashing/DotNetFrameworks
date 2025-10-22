using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Models {
	public class AgendaDbContext : IdentityDbContext<AgendaUser> {
		public DbSet<AppointmentType> AppointmentTypes {
			get; set;
		}

		public DbSet<Appointment> Appointments {
			get; set;
		}

		public DbSet<Vehicle> Vehicles {
			get; set;
		}

		public DbSet<ToDo> ToDos {
			get; set;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			// Define the connection string to the database
			string connectionString = "Server=(localdb)\\mssqllocaldb;Database=AgendaDb;Trusted_Connection=true;MultipleActiveResultSets=true";
			optionsBuilder.UseSqlServer(connectionString);
		}

		public static async Task Seeder(IServiceProvider serviceProvider) {
			var context = serviceProvider.GetRequiredService<AgendaDbContext>();
			var userManager = serviceProvider.GetRequiredService<UserManager<AgendaUser>>();

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
						Id = "Useradmin",
						Name = "Useradmin",
						NormalizedName = "USERADMIN" },
					new() {
						Id = "User",
						Name = "User",
						NormalizedName = "USER" },
					new() {
						Id = "Guest",
						Name = "Guest",
						NormalizedName = "GUEST" }
				});
				context.SaveChanges();
			}

			if (!context.Users.Any()) {
				context.Add(AgendaUser.Dummy);
				context.SaveChanges();

				await userManager.AddToRoleAsync(AgendaUser.Dummy, "Guest");

				// Default employees
				List<AgendaUser> userList = AgendaUser.SeedingData();
				foreach (var user in userList) {
					await userManager.CreateAsync(user, "P@ssword1");
				}
				await userManager.AddToRoleAsync(
					context.Users.First(ur => ur.UserName == "admin"),
					"Admin");
				await userManager.AddToRoleAsync(
					context.Users.First(ur => ur.UserName == "Bartje"),
					"Employee");
				await userManager.AddToRoleAsync(
					context.Users.First(ur => ur.UserName == "Jefke"),
					"Employee");
				await userManager.AddToRoleAsync(
					context.Users.First(ur => ur.UserName == "Dirkske"),
					"Employee");

				context.SaveChanges();
			}

			if (!context.AppointmentTypes.Any()) {
				context.AppointmentTypes.AddRange(AppointmentType.SeedingData());
				context.SaveChanges();
			}

			if (!context.Appointments.Any()) {
				context.Appointments.AddRange(Appointment.SeedingData());
				context.SaveChanges();
			}

			if (!context.Vehicles.Any()) {
				context.Vehicles.AddRange(Vehicle.SeedingData());
				context.SaveChanges();
			}

			if (!context.ToDos.Any()) {
				context.ToDos.AddRange(ToDo.SeedingData());
				context.SaveChanges();
			}
		}
	}
}