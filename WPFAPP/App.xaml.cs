using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Models;
using System.Configuration;
using System.Data;
using System.ComponentModel;
using System.Windows;

namespace WPFAPP {
	public partial class App : Application {
		static public event PropertyChangedEventHandler UserChanged = delegate { };
		static private AgendaUser _user = null!;

		static public ServiceProvider ServiceProvider {
			get; private set;
		} = null!;

		static public AgendaUser User {
			get => _user;
			set {
				if (_user != value) {
					_user = value;

					// Assumes every user has a role to it
					string currentRole = ServiceProvider
						.GetRequiredService<AgendaDbContext>().UserRoles
						.First(role => role.UserId == User.Id).RoleId;
					OnUserChanged(currentRole);
				}
			}
		}

		static public new MainWindow MainWindow {
			get; private set;
		} = null!;

		static void OnUserChanged(string role) {
			UserChanged?.Invoke(typeof(App), new PropertyChangedEventArgs(role));
		}

        // Application startup event
        protected override async void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);

			// Set the initial or "default" user
			_user = AgendaUser.Dummy;

			// Setup dependency injection
			var serviceSet = new ServiceCollection();

			// Setup DbContext as service
			serviceSet.AddLogging();
			serviceSet.AddDbContext<AgendaDbContext>();
			serviceSet.AddIdentityCore<AgendaUser>(options => {
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 3;
				options.Password.RequiredUniqueChars = 1;
			})
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<AgendaDbContext>();

			// Create the service provider which wil be accessible throughout the app
			ServiceProvider = serviceSet.BuildServiceProvider();

			// Seed the database
			await AgendaDbContext.Seeder(ServiceProvider);

			MainWindow = new(
				ServiceProvider.GetRequiredService<AgendaDbContext>(),
				ServiceProvider.GetRequiredService<UserManager<AgendaUser>>());
			MainWindow.Show();
		}
	}
}
