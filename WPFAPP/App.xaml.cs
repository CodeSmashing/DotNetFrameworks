using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Models;
using System.Configuration;
using System.Data;
using System.Windows;

namespace WPFAPP {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		static public ServiceProvider ServiceProvider {
			get; private set;
		}

		static public AgendaUser User {
			get; set;
		} = AgendaUser.Dummy;

		static public MainWindow MainWindow {
			get; private set;
		}

		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);

			// Setup dependency injection
			var serviceSet = new ServiceCollection();

			serviceSet.AddDbContext<AgendaDbContext>();

			// Setup DbContext as service
			serviceSet.AddIdentityCore<AgendaUser>(options => {
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 3;
				options.Password.RequiredUniqueChars = 1;
			})
				.AddEntityFrameworkStores<AgendaDbContext>()
				.AddRoles<IdentityRole>();

			serviceSet.AddLogging();

			// Create the service provider which wil be accessible throughout the app
			ServiceProvider = serviceSet.BuildServiceProvider();

			AgendaDbContext context = new();
			AgendaDbContext.Seeder(context);

			MainWindow = new(
				App.ServiceProvider.GetRequiredService<AgendaDbContext>(),
				App.ServiceProvider.GetRequiredService<UserManager<AgendaUser>>());
			MainWindow.Show();
		}
	}
}
