using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Models;
using System.ComponentModel;
using System.Windows;

namespace GardenPlanner_WPF {
	public class PasswordlessLoginTotpTokenProvider<TUser> : TotpSecurityStampBasedTokenProvider<TUser>
	 where TUser : class {
		public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user) {
			return Task.FromResult(false);
		}

		public override async Task<string> GetUserModifierAsync(string purpose, UserManager<TUser> manager, TUser user) {
			var email = await manager.GetEmailAsync(user);
			return "PasswordlessLogin:" + purpose + ":" + email;
		}
	}

	public static class CustomIdentityBuilderExtensions {
		public static IdentityBuilder AddPasswordlessLoginTotpTokenProvider(this IdentityBuilder builder) {
			var userType = builder.UserType;
			var totpProvider = typeof(PasswordlessLoginTotpTokenProvider<>).MakeGenericType(userType);
			return builder.AddTokenProvider("PasswordlessLoginTotpProvider", totpProvider);
		}
	}

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
		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);

			// Set the initial or "default" user
			_user = AgendaUser.Dummy;

			// Setup dependency injection
			var serviceSet = new ServiceCollection();

			// Attempt to set the services
			SetupServiceSet(serviceSet);
		}

		protected private static async void SetupServiceSet(ServiceCollection serviceSet) {
			// Clear added services of previous attempts
			if (serviceSet.Count > 0) {
				serviceSet.Clear();
			}

			try {
				// Setup DbContext as service
				var totpProvider = typeof(PasswordlessLoginTotpTokenProvider<>).MakeGenericType(typeof(AgendaUser));
				serviceSet.AddLogging()
					.AddDbContext<AgendaDbContext>()
					.AddIdentityCore<AgendaUser>(options => {
						options.Password.RequireDigit = false;
						options.Password.RequireLowercase = false;
						options.Password.RequireNonAlphanumeric = false;
						options.Password.RequireUppercase = false;
						options.Password.RequiredLength = 3;
						options.Password.RequiredUniqueChars = 1;
						options.Tokens.PasswordResetTokenProvider = "PasswordlessLoginTotpProvider";
					})
					.AddRoles<IdentityRole>()
					.AddEntityFrameworkStores<AgendaDbContext>()
					.AddPasswordlessLoginTotpTokenProvider();

				// Create the service provider which wil be accessible throughout the app
				ServiceProvider = serviceSet.BuildServiceProvider();

				// Seed the database
				await AgendaDbContext.Seeder(ServiceProvider);

				MainWindow = new(
					ServiceProvider.GetRequiredService<AgendaDbContext>(),
					ServiceProvider.GetRequiredService<UserManager<AgendaUser>>());
				MainWindow.Show();
			} catch (Exception ex) {
				// Show the error message(s) to tell the user what went wrong
				MessageBoxResult result = MessageBox.Show(
					 $"Error: {ex.Message}\n" +
					"Wilt u de applicatie sluiten?",
					 "Fout details-scherm applicatie",
					 MessageBoxButton.YesNo,
					 MessageBoxImage.Error
				);

				if (result == MessageBoxResult.Yes) {
					Current.Shutdown();
				} else {
					// EF Core delays this through their retry attempt time
					SetupServiceSet(serviceSet);
				}
			}
		}
	}
}
