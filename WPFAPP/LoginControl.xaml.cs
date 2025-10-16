using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFAPP {
	/// <summary>
	/// Interaction logic for LoginControl.xaml
	/// </summary>
	public partial class LoginControl : UserControl {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		public event EventHandler SwapRequested = null!;

		public LoginControl(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();
		}

		private async void btnLogin_Click(object sender, RoutedEventArgs e) {
			// Return early if inputs are empty
			if (string.IsNullOrEmpty(tbUsername.Text)) {
				tbError.Text = "Username is required";
				return;
			}

			if (string.IsNullOrEmpty(pbPassword.Password)) {
				tbError.Text = "Password is required";
				return;
			}

			if (!string.IsNullOrEmpty(pbPassword.Password) && !string.IsNullOrEmpty(tbUsername.Text)) {
				AgendaUser? user = await _userManager.FindByNameAsync(tbUsername.Text);

				if (user != null) {
					bool loginSuccess = await _userManager.CheckPasswordAsync(user, pbPassword.Password);

					if (loginSuccess) {
						// Update UI for logged in user
						App.User = user;

						// Return to appointments tab
						App.MainWindow.tcNavigation.SelectedItem = App.MainWindow.tciAppointmentRequest;

						// Show/hide relevant controls
						App.MainWindow.dgAppointments.Visibility = Visibility.Visible;
						App.MainWindow.tciGeneral.Visibility = Visibility.Visible;
						App.MainWindow.tciRegisterLogin.Visibility = Visibility.Hidden;
						App.MainWindow.btnLogout.Visibility = Visibility.Visible;
						App.MainWindow.tbUsernameInfo.Text = user.UserName?.ToString();

						IdentityUserRole<string>? isUserAdmin = _context.UserRoles.FirstOrDefault(ur => ur.UserId == App.User.Id && ur.RoleId == "UserAdmin");
						if (isUserAdmin != null) {
							App.MainWindow.tciUsers.Visibility = Visibility.Visible;
						} else {
							App.MainWindow.tciUsers.Visibility = Visibility.Hidden;
						}
					}
					tbError.Text = "Invalid username or password";
				} else {
					tbError.Text = "Invalid username or password";
				}
			}
		}

		// Notify MainWindow to swap to whatever control it wants
		private void btnSwap_Click(object sender, RoutedEventArgs e) {
			SwapRequested?.Invoke(this, EventArgs.Empty);
		}
	}
}
