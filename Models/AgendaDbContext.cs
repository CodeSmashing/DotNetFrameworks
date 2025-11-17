using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<AgendaUser>()
				.HasIndex(u => u.Email)
				.IsUnique();

			modelBuilder.Entity<AgendaUser>()
				.HasIndex(u => u.DisplayName)
				.IsUnique();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			// Define the connection string to the database
			string connectionString = "Server=(localdb)\\mssqllocaldb;Database=AgendaDb;Trusted_Connection=true;MultipleActiveResultSets=true";

			if (!optionsBuilder.IsConfigured) {
				try {
					var config = new ConfigurationBuilder()
						.SetBasePath(AppContext.BaseDirectory)  // The directory of the Model library, not the executing project
						.AddJsonFile("appsettings.json", optional: true) // Get the connection string from the Json-file
						.AddUserSecrets<AgendaDbContext>(optional: true) // Add the User Secrets
						.AddEnvironmentVariables()
						.Build();
					string? con = config.GetConnectionString("ServerConnection");

					if (!con.IsNullOrEmpty())
						connectionString = con;
				} catch {
					throw;
				}
			}

			optionsBuilder.UseSqlServer(connectionString, options => {
				options.EnableRetryOnFailure(
					 maxRetryCount: 3,
					 maxRetryDelay: TimeSpan.FromSeconds(5),
					 errorNumbersToAdd: [4060]
				);
			});
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
						Id = "UserAdmin",
						Name = "UserAdmin",
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
				AgendaUser[] userList = AgendaUser.SeedingData();
				foreach (var user in userList) {
					await userManager.CreateAsync(user, "P@ssword1");
				}
				await userManager.AddToRoleAsync(AgendaUser.Dummy, "Guest");
				await userManager.AddToRoleAsync(
					context.Users.First(ur => ur.Email == "admin.bob@gardenDb.org"),
					"Admin");
				await userManager.AddToRoleAsync(
					context.Users.First(ur => ur.Email == "employee.bart@gardenDb.org"),
					"Employee");
				await userManager.AddToRoleAsync(
					context.Users.First(ur => ur.Email == "employee.jeff@gardenDb.org"),
					"Employee");
				await userManager.AddToRoleAsync(
					context.Users.First(ur => ur.Email == "employee.dirk@gardenDb.org"),
					"Employee");
				await userManager.AddToRoleAsync(
					context.Users.First(ur => ur.Email == "useradmin.wim@gardenDb.org"),
					"UserAdmin");
				await userManager.AddToRoleAsync(
					context.Users.First(ur => ur.Email == "jeff.broek@gmail.com"),
					"User");
				await userManager.AddToRoleAsync(
					context.Users.First(ur => ur.Email == "rosse.emanuel@hotmail.com"),
					"User");
				await userManager.AddToRoleAsync(
					context.Users.First(ur => ur.Email == "bartbartbart@gmail.com"),
					"User");
				context.SaveChanges();
			}

			if (!context.AppointmentTypes.Any()) {
				context.AppointmentTypes.AddRange(AppointmentType.SeedingData());
				context.SaveChanges();
			}

			if (!context.Appointments.Any()) {
				// Only use regular users
				context.Appointments.AddRange(Appointment.SeedingData(
					context.UserRoles
						.Where(ur => ur.RoleId == "User")
						.Select(ur => ur.UserId)
						.Join(context.Users,
							userRoleUserId => userRoleUserId,
							user => user.Id,
							(userRoleUserId, user) => user.Id)
						.ToArray(),

					context.AppointmentTypes
						.Select(appt => appt.Id)
						.ToArray()
				));
				context.SaveChanges();
			}

			if (!context.Vehicles.Any()) {
				context.Vehicles.AddRange(Vehicle.SeedingData());
				context.SaveChanges();
			}

			if (!context.ToDos.Any()) {
				context.ToDos.AddRange(ToDo.SeedingData(
					context.Appointments
						.Select(app => app.Id)
						.ToArray()));
				context.SaveChanges();
			}
		}
	}
}