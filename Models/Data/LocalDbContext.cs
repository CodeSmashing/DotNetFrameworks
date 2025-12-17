using Microsoft.EntityFrameworkCore;

namespace Models {
	/// <summary>
	/// De databasecontext voor de lokale SQLite-omgeving, verantwoordelijk voor synchronisatie en offline gegevensopslag.
	/// </summary>
	public class LocalDbContext : DbContext {
		/// <summary>
		/// Haalt de verzameling van <see cref="LocalAppointmentType"/> entiteiten op of stelt deze in.
		/// Deze set vertegenwoordigt de lokale tabel voor de gesynchroniseerde modellen.
		/// </summary>
		public DbSet<LocalAppointmentType> AppointmentTypes {
			get; set;
		}

		/// <summary>
		/// Haalt de verzameling van <see cref="LocalAppointment"/> entiteiten op of stelt deze in.
		/// Deze set vertegenwoordigt de lokale tabel voor de gesynchroniseerde modellen.
		/// </summary>
		public DbSet<LocalAppointment> Appointments {
			get; set;
		}

		/// <summary>
		/// Haalt de verzameling van <see cref="LocalVehicle"/> entiteiten op of stelt deze in.
		/// Deze set vertegenwoordigt de lokale tabel voor de gesynchroniseerde modellen.
		/// </summary>
		public DbSet<LocalVehicle> Vehicles {
			get; set;
		}

		/// <summary>
		/// Haalt de verzameling van <see cref="AgendaUser"/> entiteiten op of stelt deze in.
		/// Deze set vertegenwoordigt de lokale tabel voor de gesynchroniseerde modellen.
		/// </summary>
		public DbSet<AgendaUser> Users {
			get; set;
		}

		/// <summary>
		/// Haalt de verzameling van <see cref="LocalToDo"/> entiteiten op of stelt deze in.
		/// Deze set vertegenwoordigt de lokale tabel voor de gesynchroniseerde modellen.
		/// </summary>
		public DbSet<LocalToDo> ToDos {
			get; set;
		}

		/// <summary>
		/// Haalt de verzameling van <see cref="Language"/> entiteiten op of stelt deze in.
		/// Deze set vertegenwoordigt de lokale tabel voor de gesynchroniseerde modellen.
		/// </summary>
		public DbSet<Language> Languages {
			get; set;
		}

		/// <summary>
		/// Haalt de verzameling van <see cref="Login"/> entiteiten op of stelt deze in.
		/// Deze set vertegenwoordigt de lokale tabel voor de gesynchroniseerde modellen.
		/// </summary>
		public DbSet<Login> Logins {
			get; set;
		}

		/// <summary>
		/// Wordt aangeroepen om de databaseverbinding en andere configuratieopties in te stellen.
		/// Wordt hier specifiek gebruikt voor de configuratie van de lokale SQLite-verbinding.
		/// </summary>
		/// <param name="options">Een builder die wordt gebruikt om de opties voor de context te maken of te wijzigen.</param>
		protected override void OnConfiguring(DbContextOptionsBuilder options) {
			var folder = Environment.SpecialFolder.LocalApplicationData;
			var path = Environment.GetFolderPath(folder);
			var DbPath = System.IO.Path.Join(path, "GardenPlanner_Local.db");
			options.UseSqlite($"Data Source={DbPath}");
		}
	}
}