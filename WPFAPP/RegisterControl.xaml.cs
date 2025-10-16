using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
	/// Interaction logic for RegisterControl.xaml
	/// </summary>
	public partial class RegisterControl : UserControl {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		public event EventHandler SwapRequested = null!;

		public RegisterControl(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();
		}

		private void btnRegister_Click(object sender, RoutedEventArgs e) {
			if (string.IsNullOrEmpty(tbUsername.Text)) {
				tbError.Text = "Username is required";
				return;
			}
			if (string.IsNullOrEmpty(tbFirstName.Text)) {
				tbError.Text = "First name is required";
				return;
			}
			if (string.IsNullOrEmpty(tbLastName.Text)) {
				tbError.Text = "Last name is required";
				return;
			}
			if (string.IsNullOrEmpty(tbEmail.Text)) {
				tbError.Text = "Email is required";
				return;
			}
			if (string.IsNullOrEmpty(pbPassword.Password)) {
				tbError.Text = "Password is required";
				return;
			}
			if (pbPassword.Password != pbConfirmPassword.Password) {
				tbError.Text = "Passwords aren't the same";
				return;
			}

			AgendaUser registeredUser = new() {
				UserName = tbUsername.Text,
				FirstName = tbFirstName.Text,
				LastName = tbLastName.Text,
				Email = tbEmail.Text,
				EmailConfirmed = true,
				LockoutEnabled = false,
				TwoFactorEnabled = false
			};
			var result = _userManager.CreateAsync(registeredUser, pbPassword.Password).Result;

			if (result.Succeeded) {
				MessageBox.Show("Account created successfully. You can now log in.");
				tbError.Text = "User registered successfully";

				_context.Add(new IdentityUserRole<string>() { RoleId = "User", UserId = registeredUser.Id });
				_context.SaveChanges();

				// Update UI for logged in user
				App.User = registeredUser;

				// Return to appointments tab
				App.MainWindow.tcNavigation.SelectedItem = App.MainWindow.tciAppointmentRequest;

				// Show/hide relevant controls
				App.MainWindow.dgAppointments.Visibility = Visibility.Visible;
				App.MainWindow.tciGeneral.Visibility = Visibility.Visible;
				App.MainWindow.tciRegisterLogin.Visibility = Visibility.Hidden;
				App.MainWindow.btnLogout.Visibility = Visibility.Visible;
				App.MainWindow.tbUsernameInfo.Text = registeredUser.UserName?.ToString();
			} else {
				tbError.Text = string.Join("\n", result.Errors.Select(err => err.Description));
				return;
			}
		}

		// Notify MainWindow to swap to whatever control it wants
		private void btnSwap_Click(object sender, RoutedEventArgs e) {
			SwapRequested?.Invoke(this, EventArgs.Empty);
		}
	}
}
