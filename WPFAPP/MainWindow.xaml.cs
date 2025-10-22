using Microsoft.AspNetCore.Identity;
using Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFAPP {
	public partial class MainWindow : Window {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		private readonly LoginControl loginControl;
		private readonly RegisterControl registerControl;
		private readonly RoleControl roleControl;
		private readonly AdminControl adminControl;
		private readonly AppointmentControl appointmentControl;

		public MainWindow(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();

			// Instantiate controls and their containers
			loginControl = new(_context, _userManager);
			registerControl = new(_context, _userManager);
			roleControl = new(_context, _userManager);
			adminControl = new(_context, _userManager);

			FormContainer.Children.Clear();
			FormContainer.Children.Add(registerControl);
			UserRoleInfoContainer.Children.Add(roleControl);
			AdminPanelContainer.Children.Add(adminControl);
			appointmentControl = new(_context, _userManager);
			AppointmentContainer.Children.Clear();
			AppointmentContainer.Children.Add(appointmentControl);

			// Subscribe to login and register swap events
			loginControl.SwapRequested += SwapControlsHandler;
			registerControl.SwapRequested += SwapControlsHandler;

			// Subscribe to login and register success events
			loginControl.LoginSuccess += SuccessfulLoginRegisterHandler;
			registerControl.RegisterSuccess += SuccessfulLoginRegisterHandler;

			// Subscribe to app user change event
			App.UserChanged += HandleUserChanged;

			// Initially show the guest UI
			DisplayUI("guest");
		}

		public void HandleUserChanged(object? sender, PropertyChangedEventArgs e) {
			DisplayUI(e.PropertyName);
		}

		private void btnLogout_Click(object sender, RoutedEventArgs e) {
			// Show/hide relevant elements
			App.User = AgendaUser.Dummy;

			// Ensure buttons are updated for logged out state
			UpdateButtonsVisibilityForSelectedRow();

			// Return to appointments tab
			tcNavigation.SelectedItem = tciAppointmentRequest;
		}

		private void SwapControlsHandler(object? sender, EventArgs e) {
			// Swap between login and register controls
			if (FormContainer.Children.Contains(registerControl)) {
				// Switch to login form
				FormContainer.Children.Clear();
				FormContainer.Children.Add(loginControl);
			} else {
				// Switch to register form
				FormContainer.Children.Clear();
				FormContainer.Children.Add(registerControl);
			}
		}

		private void SuccessfulLoginRegisterHandler(object? sender, AgendaUser user) {
			// Set the current user of the app
			App.User = user;

			// Go to the appointments tab
			tcNavigation.SelectedItem = tciAppointmentTab;
		}

		public void DisplayUI(string? role = "") {
			switch (role) {
				case "User":
					tciRegisterLogin.Visibility = Visibility.Hidden;
					tciGeneral.Visibility = Visibility.Visible;
					btnLogout.Visibility = Visibility.Visible;
					tciUsers.Visibility = Visibility.Hidden;
					tbUsernameInfo.Text = App.User.UserName?.ToString();
					tciAppointmentTab.Visibility = Visibility.Visible;
					break;
				case "Admin":
				case "Useradmin":
				case "Employee":
					tciRegisterLogin.Visibility = Visibility.Hidden;
					tciGeneral.Visibility = Visibility.Visible;
					btnLogout.Visibility = Visibility.Visible;
					tciUsers.Visibility = Visibility.Visible;
					tbUsernameInfo.Text = App.User.UserName?.ToString();
					tciAppointmentTab.Visibility = Visibility.Visible;
					break;
				case "Guest":
				default:
					tciRegisterLogin.Visibility = Visibility.Visible;
					tciGeneral.Visibility = Visibility.Hidden;
					btnLogout.Visibility = Visibility.Hidden;
					tciUsers.Visibility = Visibility.Hidden;
					tbUsernameInfo.Text = string.Empty;
					tcNavigation.SelectedItem = tciAuthenticationTab;
					tciAppointmentTab.Visibility = Visibility.Hidden;
					break;
			}
		}
	}
}