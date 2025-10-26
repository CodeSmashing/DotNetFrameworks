using Microsoft.AspNetCore.Identity;
using Models;
using System.ComponentModel;
using System.Windows;

namespace WPFAPP {
	public partial class MainWindow : Window {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		private readonly LoginControl loginControl;
		private readonly RegisterControl registerControl;
		private readonly UserInfoControl userControl;
		private readonly AppointmentControl appointmentControl;
		private readonly AdminPanelControl adminPanelControl;

		public MainWindow(AgendaDbContext context, UserManager<AgendaUser> userManager) {

			Application.Current.MainWindow.WindowState = WindowState.Maximized;

			_context = context;
			_userManager = userManager;
			InitializeComponent();

			// Instantiate controls and their containers
			loginControl = new(_context, _userManager);
			registerControl = new(_context, _userManager);
			userControl = new();
			appointmentControl = new(_context, _userManager);
			adminPanelControl = new(_context, _userManager);
			AuthenticationContainer.Children.Clear();
			UserInfoContainer.Children.Clear();
			AppointmentContainer.Children.Clear();
			AuthenticationContainer.Children.Add(registerControl);
			UserInfoContainer.Children.Add(userControl);
			AppointmentContainer.Children.Add(appointmentControl);
			AdminPanelContainer.Children.Add(adminPanelControl);

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

		private void SwapControlsHandler(object? sender, EventArgs e) {
			// Swap between login and register controls
			if (AuthenticationContainer.Children.Contains(registerControl)) {
				// Switch to login form
				AuthenticationContainer.Children.Clear();
				AuthenticationContainer.Children.Add(loginControl);
			} else {
				// Switch to register form
				AuthenticationContainer.Children.Clear();
				AuthenticationContainer.Children.Add(registerControl);
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
					tciAppointmentTab.Visibility = Visibility.Visible;
					tciAuthenticationTab.Visibility = Visibility.Hidden;
					tciUserInfoTab.Visibility = Visibility.Visible;
					tciAdminPanelTab.Visibility = Visibility.Hidden;
					break;
				case "Admin":
				case "Useradmin":
				case "Employee":
					tciAppointmentTab.Visibility = Visibility.Visible;
					tciAuthenticationTab.Visibility = Visibility.Hidden;
					tciUserInfoTab.Visibility = Visibility.Visible;
					tciAdminPanelTab.Visibility = Visibility.Visible;
					break;
				case "Guest":
				default:
					tcNavigation.SelectedItem = tciAuthenticationTab;
					tciAppointmentTab.Visibility = Visibility.Hidden;
					tciAuthenticationTab.Visibility = Visibility.Visible;
					tciUserInfoTab.Visibility = Visibility.Hidden;
					tciAdminPanelTab.Visibility = Visibility.Hidden;
					break;
			}
		}
	}
}