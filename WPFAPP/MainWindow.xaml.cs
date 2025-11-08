using Microsoft.AspNetCore.Identity;
using Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WPFAPP {
	public partial class MainWindow : Window {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		private readonly LoginControl loginControl;
		private readonly RegisterControl registerControl;
		private readonly UserInfoControl userInfoControl;
		private readonly AppointmentControl appointmentControl;
		private readonly AdminPanelControl adminPanelControl;

		public MainWindow(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			Application.Current.MainWindow.WindowState = WindowState.Maximized;

			_context = context;
			_userManager = userManager;
			InitializeComponent();

			// Subscribe to app user change event
			App.User = _context.Users.First(u => u.Email == "admin.bob@GardenDb.org");
			DisplayUI(this, new PropertyChangedEventArgs("Admin"));


			// Instantiate controls and their containers
			loginControl = new(_context, _userManager);
			registerControl = new(_context, _userManager);
			userInfoControl = new();
			appointmentControl = new(_context);
			adminPanelControl = new(_context, _userManager);
			tciAuthenticationTab.Content = registerControl;
			tciUserInfoTab.Content = userInfoControl;
			tciAppointmentTab.Content = appointmentControl;
			tciAdminPanelTab.Content = adminPanelControl;

			// Subscribe to login and register swap events
			loginControl.SwapRequested += SwapControlsHandler;
			registerControl.SwapRequested += SwapControlsHandler;

			// Subscribe to login and register success events
			loginControl.LoginSuccess += SuccessfulLoginRegisterHandler;
			registerControl.RegisterSuccess += SuccessfulLoginRegisterHandler;

			// Subscribe to app user change event
			App.UserChanged += DisplayUI;

			// Initially show the guest UI
			//DisplayUI(this, new PropertyChangedEventArgs(string.Empty));
		}

		private void SwapControlsHandler(object? sender, EventArgs e) {
			// Swap between login and register controls
			if (tciAuthenticationTab.Content == registerControl) {
				// Switch to login form
				tciAuthenticationTab.Content = loginControl;
			} else {
				// Switch to register form
				tciAuthenticationTab.Content = registerControl;
			}
		}

		private void SuccessfulLoginRegisterHandler(object? sender, AgendaUser user) {
			// Set the current user of the app
			App.User = user;

			// Go to the appointments tab
			tcNavigation.SelectedItem = tciAppointmentTab;
		}

		public void DisplayUI(object? sender, PropertyChangedEventArgs e) {
			string? role = e.PropertyName;
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

		public static bool ValidateInputs(out List<Control> errors, in Dictionary<Control, string> inputRequirements) {
			errors = new();
			bool isValid = true;
			foreach (Control field in inputRequirements.Keys) {
				bool isInValid = false;
				switch (field) {
					case TextBox textBox:
						isInValid = textBox.Text.Length == 0;
						break;
					case ComboBox comboBox:
						isInValid = comboBox.SelectedIndex == -1;
						break;
					case DatePicker datePicker:
						isInValid = datePicker.SelectedDate <= DateTime.Now || !datePicker.SelectedDate.HasValue;
						break;
				}
				if (isInValid) {
					isValid = false;
					errors.Add(field);
				}
			}
			return isValid;
		}
	}
}