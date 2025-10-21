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
		} = null!;

		static public AgendaUser User {
			get; set;
		} = AgendaUser.Dummy;

		static public new MainWindow MainWindow {
			get; private set;
		} = null!;

		protected override async void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);

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
