using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Models {
	/// <summary>
	/// De databasecontext voor de globale GardenPlanner SQL-omgeving, verantwoordelijk voor de
	/// online gegevensopslag.
	/// Beheert de toegang tot afspraken, voertuigen en gerelateerde gegevens.
	/// </summary>
	public class GlobalDbContext : IdentityDbContext<AgendaUser> {
		/// <summary>
		/// De verschillende types afspraken die beschikbaar zijn in het systeem.
		/// </summary>
		public DbSet<AppointmentType> AppointmentTypes {
			get; set;
		}

		/// <summary>
		/// De geplande afspraken van de gebruikers.
		/// </summary>
		public DbSet<Appointment> Appointments {
			get; set;
		}

		/// <summary>
		/// De voertuigen die geregistreerd staan voor afspraken of onderhoud.
		/// </summary>
		public DbSet<Vehicle> Vehicles {
			get; set;
		}

		/// <summary>
		/// De lijst met taken of to-do items.
		/// </summary>
		public DbSet<ToDo> ToDos {
			get; set;
		}

		/// <summary>
		/// De ondersteunde talen voor lokalisatie binnen de applicatie.
		/// </summary>
		public DbSet<Language> Languages {
			get; set;
		}

		/// <summary>
		/// Initialiseert een nieuwe instantie van de <see cref="GlobalDbContext"/> klasse
		/// met standaard opties.
		/// </summary>
		public GlobalDbContext() : base() { }

		/// <summary>
		/// Initialiseert een nieuwe instantie van de <see cref="GlobalDbContext"/> klasse
		/// met de opgegeven configuratie.
		/// </summary>
		/// <param name="options">
		/// De configuratie-opties voor deze database context.
		/// </param>
		public GlobalDbContext(DbContextOptions<GlobalDbContext> options) : base(options) { }

		/// <summary>
		/// Configureert het databasemodel en de entiteitsrelaties via de Fluent API.
		/// Hier worden zaken als primaire sleutels, indexen en zaadgegevens (seed data) gedefinieerd.
		/// </summary>
		/// <param name="modelBuilder">
		/// De builder die wordt gebruikt om het model te construeren.
		/// </param>
		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<AgendaUser>()
				.HasIndex(u => u.Email)
				.IsUnique();

			modelBuilder.Entity<AgendaUser>()
				.HasIndex(u => u.DisplayName)
				.IsUnique();
		}

		/// <summary>
		/// Configureert de databaseverbinding en andere opties voor de context.
		/// Wordt aangeroepen als de context niet via Dependency Injection is geconfigureerd.
		/// </summary>
		/// <param name="optionsBuilder">
		/// De builder voor de configuratie-opties van de context.
		/// </param>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			// Define the connection string to the database
			string connectionString = "Server=(localdb)\\mssqllocaldb;Database=AgendaDb;Trusted_Connection=true;MultipleActiveResultSets=true";

			if (!optionsBuilder.IsConfigured) {
				try {
					var config = new ConfigurationBuilder()
						.SetBasePath(AppContext.BaseDirectory)  // The directory of the Model library, not the executing project
						.AddJsonFile("appsettings.json", optional: true) // Get the connection string from the Json-file
						.AddUserSecrets<GlobalDbContext>(optional: true) // Add the User Secrets
						.AddEnvironmentVariables()
						.Build();
					string? con = config.GetConnectionString("ServerConnection");

					if (!string.IsNullOrEmpty(con)) {
						connectionString = con;
					}
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

		/// <summary>
		/// Voegt initiële data (seed data) toe aan de database indien deze leeg is.
		/// </summary>
		/// <param name="serviceProvider">
		/// De service provider voor het ophalen van benodigde services zoals de database context.
		/// </param>
		/// <returns>
		/// Een task die de asynchrone operatie representeert.
		/// </returns>
		public static async Task Seeder(IServiceProvider serviceProvider) {
			var context = serviceProvider.GetRequiredService<GlobalDbContext>();
			var userManager = serviceProvider.GetRequiredService<UserManager<AgendaUser>>();

			if (!context.Languages.Any()) {
				context.Languages.AddRange(Language.SeedingData());
				context.SaveChanges();
			}

			// A list to lessen database queries
			Language.Languages.AddRange(context.Languages
				.Where(l => l.IsActive)
				.OrderBy(l => l.Name)
				.ToList());

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
			} else {
				AgendaUser.Dummy = context.Users.First(u => u.Id == "-");
			}

			if (!context.AppointmentTypes.Any()) {
				context.AppointmentTypes.AddRange(AppointmentType.SeedingData());
				context.SaveChanges();
			} else {
				AppointmentType.Dummy = context.AppointmentTypes.First(appt => appt.Id == "-");
			}

			if (!context.Appointments.Any()) {
				context.Appointments.AddRange(Appointment.SeedingData(
					// Only use regular users
					context.Users
						.Where(user => context.UserRoles
							.Any(ur => ur.UserId == user.Id && ur.RoleId == "User"))
						.Select(u => u.Id)
						.ToArray(),

					// Non dummy data
					context.AppointmentTypes
						.Where(appt => appt.Id != "-")
						.Select(appt => appt.Id)
						.ToArray()
				));
				context.SaveChanges();
			} else {
				Appointment.Dummy = context.Appointments.First(app => app.Id == "-");
			}

			if (!context.Vehicles.Any()) {
				context.Vehicles.AddRange(Vehicle.SeedingData());
				context.SaveChanges();
			} else {
				Vehicle.Dummy = context.Vehicles.First(v => v.Id == "-");
			}

			if (!context.ToDos.Any()) {
				context.ToDos.AddRange(ToDo.SeedingData(
					// Non dummy data
					context.Appointments
						.Where(appt => appt.Id != "-")
						.Select(app => app.Id)
						.ToArray()));
				context.SaveChanges();
			} else {
				ToDo.Dummy = context.ToDos.First(td => td.Id == "-");
			}
		}
	}
}