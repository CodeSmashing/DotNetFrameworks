using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Models {
	public class AgendaDbContext : IdentityDbContext<AgendaUser> {
		public DbSet<AppointmentType> AppointmentTypes {
			get; set;
		}

		public DbSet<Appointment> Appointments {
			get; set;
		}

		public DbSet<ToDo> ToDos {
			get; set;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			string connectionString = "Server=(localdb)\\mssqllocaldb;Database=AgendaDb;Trusted_Connection=true;MultipleActiveResultSets=true";

			optionsBuilder.UseSqlServer(connectionString);
		}

		public static async Task Seeder(AgendaDbContext context) {
			UserManager<AgendaUser> userManager = new(
				new UserStore<AgendaUser>(context),
				null,
				new PasswordHasher<AgendaUser>(),
				null, null, null, null, null, null
			);

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
						NormalizedName = "USER" }
				});
				context.SaveChanges();
			}

			if (!context.Users.Any()) {
				context.Add(AgendaUser.Dummy);
				context.SaveChanges();

				List<AgendaUser> userList = AgendaUser.SeedingData();
				foreach (var user in userList) {
					await userManager.CreateAsync(user, "P@ssword1");
					await userManager.AddToRoleAsync(user, user.UserName.ElementAt(0) + user.UserName.Substring(1));
				}
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

			if (!context.ToDos.Any()) {
				context.ToDos.AddRange(ToDo.SeedingData());
				context.SaveChanges();
			}
	  }
	}
}