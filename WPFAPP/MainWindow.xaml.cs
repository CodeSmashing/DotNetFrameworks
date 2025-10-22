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

		public MainWindow(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();

			// Instantiate controls and their containers
			loginControl = new(_context, _userManager);
			registerControl = new(_context, _userManager);
			roleControl = new(_context, _userManager);
			FormContainer.Children.Clear();
			FormContainer.Children.Add(registerControl);
			UserRoleInfoContainer.Children.Add(roleControl);

			// Subscribe to login and register swap events
			loginControl.SwapRequested += SwapControlsHandler;
			registerControl.SwapRequested += SwapControlsHandler;

			// Subscribe to login and register success events
			loginControl.LoginSuccess += SuccessfulLoginRegisterHandler;
			registerControl.RegisterSuccess += SuccessfulLoginRegisterHandler;

			// Subscribe to app user change event
			App.UserChanged += HandleUserChanged;

			// Subscribe to other events
			dgAppointments.MouseDoubleClick += dgAppointments_MouseDoubleClick;

			// Initially show the guest UI
			DisplayUI("guest");

			// Load appointments data
			UpdateDgAppointments();

			// Subscribe to double-click event on DataGrid rows
			dgAppointments.MouseDoubleClick += dgAppointments_MouseDoubleClick;

			// Load appointment types into combobox
			cbTypes.ItemsSource = _context.AppointmentTypes.ToList();
		}

		public void HandleUserChanged(object? sender, PropertyChangedEventArgs e) {
			DisplayUI(e.PropertyName);
		}

		private void dgAppointments_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if (dgAppointments.SelectedItem is Appointment selectedAppointment) {
				// Show appointment details in a popup
				MessageBox.Show(
					 $"From: {selectedAppointment.From}\n" +
					 $"To: {selectedAppointment.To}\n" +
					 $"Title: {selectedAppointment.Title}\n" +
					 $"Description: {selectedAppointment.Description}\n" +
					 $"Type: {selectedAppointment.AppointmentType}",
					 "Appointment Details",
					 MessageBoxButton.OK,
					 MessageBoxImage.Information
				);
			}
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

		private void dgAppointments_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			grDetails.Visibility = Visibility.Hidden;
			btnSave.Visibility = Visibility.Hidden;

			// Existing index-based logic removed in favour of clearer helper
			UpdateButtonsVisibilityForSelectedRow();
		}

		// Add, Edit, Delete, Save button handlers
		private void btnAdd_Click(object sender, RoutedEventArgs e) {
			btnSave.Visibility = Visibility.Visible;
			grDetails.Visibility = Visibility.Visible;
			grDetails.DataContext = new Appointment();
		}
		private void btnEdit_Click(object sender, RoutedEventArgs e) {
			if (btnEdit.Content.ToString() == "Wijzigen") {
				btnEdit.Content = "Opslaan";
			} else if (btnEdit.Content.ToString() == "Opslaan") {
				btnEdit.Content = "Wijzigen";
			}

			btnSave.Visibility = Visibility.Hidden;
			grDetails.Visibility = Visibility.Visible;
			grDetails.DataContext = dgAppointments.SelectedItem;
		}
		private void btnDelete_Click(object sender, RoutedEventArgs e) {
			try {
				Appointment appointment = (Appointment) dgAppointments.SelectedItem;
				Appointment? contextAppointment = _context.Appointments.FirstOrDefault(app => app.Id == appointment.Id);

				if (contextAppointment != null) {
					contextAppointment.Deleted = DateTime.Now;
					_context.SaveChanges();

					UpdateDgAppointments();
				}
			} catch (Exception errorInfo) {
				Console.WriteLine("Fout bij verwijderen afspraak; " + errorInfo.Message);
			}
		}

		private void btnSave_Click(object sender, RoutedEventArgs e) {
			try {
				Appointment appointment = new();
				Appointment contextAppointment = (Appointment) grDetails.DataContext;

				appointment.UserId = App.User.Id;
				appointment.From = contextAppointment.From;
				appointment.To = contextAppointment.To;
				appointment.Title = contextAppointment.Title;
				appointment.Description = contextAppointment.Description;
				appointment.AppointmentType = contextAppointment.AppointmentType;
				appointment.AppointmentTypeId = contextAppointment.AppointmentType.Id;

				if (appointment.AppointmentType == AppointmentType.Dummy) {
					throw new Exception("Ongeldig type afspraak geselecteerd.");
				}

				// Save to database
				_context.Appointments.Add(appointment);
				_context.SaveChanges();
				btnSave.Visibility = Visibility.Hidden;
				grDetails.Visibility = Visibility.Hidden;

				// Refresh appointments DataGrid
				UpdateDgAppointments();

			} catch (Exception erorrInfo) {
				Console.WriteLine("Fout bij opslaan afspraak; " + erorrInfo.Message);
			}
		}

		// Handlers to show Save button when details are changed
		private void SelectedDateChanged(object sender, SelectionChangedEventArgs e) {
			//btnSave.Visibility = Visibility.Visible;
		}

		private void TextChanged(object sender, TextChangedEventArgs e) {
			//btnSave.Visibility = Visibility.Visible;
		}

		// Refresh the appointments DataGrid with current data from the database
		private void UpdateDgAppointments() {
			dgAppointments.ItemsSource = _context.Appointments
										.Where(app => app.Deleted >= DateTime.Now
													  && app.From > DateTime.Now
													  && app.UserId == App.User.Id)
										.OrderBy(app => app.From)
										.Select(app => app)
										//.Include(app => app.AppointmentType)  // Eager loading van AppointmentType
										.ToList();

			// After reloading the items, update the button visibility
			UpdateButtonsVisibilityForSelectedRow();
		}

		private void SuccessfulLoginRegisterHandler(object? sender, AgendaUser user) {
			// Set the current user of the app
			App.User = user;

			// Update appointments DataGrid
			UpdateDgAppointments();

			// Return to appointments tab
			tcNavigation.SelectedItem = tciAppointmentRequest;

			// Ensure button visibility is correct after successful login
			UpdateButtonsVisibilityForSelectedRow();
		}

		// Helper to update button visibility based on selected appointment and user state
		private void UpdateButtonsVisibilityForSelectedRow() {
			// Default states
			btnEdit.Visibility = Visibility.Hidden;
			btnDelete.Visibility = Visibility.Hidden;
			btnEdit.IsEnabled = false;
			btnDelete.IsEnabled = false;

			// btnAdd visible when a real user is logged in
			btnAdd.Visibility = (App.User != null && App.User != AgendaUser.Dummy) ? Visibility.Visible : Visibility.Hidden;

			// If no selection, nothing more to do
			if (dgAppointments.SelectedItem == null || dgAppointments.SelectedIndex < 0) {
				return;
			}

			// If the selected item is an Appointment, evaluate ownership and validity
			if (dgAppointments.SelectedItem is Appointment selectedAppointment) {
				// If dummy or deleted already -> keep hidden/disabled
				if (selectedAppointment == Appointment.Dummy || selectedAppointment.Deleted <= DateTime.Now) {
					return;
				}

				// Only allow edit/delete for appointments belonging to the logged-in user
				if (App.User != null && App.User != AgendaUser.Dummy && selectedAppointment.UserId == App.User.Id) {
					btnEdit.Visibility = Visibility.Visible;
					btnDelete.Visibility = Visibility.Visible;
					btnEdit.IsEnabled = true;
					btnDelete.IsEnabled = true;
				}
			}
		}

		public void DisplayUI(string? role = "") {
			switch (role) {
				case "User":
					dgAppointments.Visibility = Visibility.Visible;
					tciRegisterLogin.Visibility = Visibility.Hidden;
					tciGeneral.Visibility = Visibility.Visible;
					btnLogout.Visibility = Visibility.Visible;
					tciUsers.Visibility = Visibility.Hidden;
					tbUsernameInfo.Text = App.User.UserName?.ToString();
					break;
				case "Admin":
				case "Useradmin":
				case "Employee":
					dgAppointments.Visibility = Visibility.Visible;
					tciRegisterLogin.Visibility = Visibility.Hidden;
					tciGeneral.Visibility = Visibility.Visible;
					btnLogout.Visibility = Visibility.Visible;
					tciUsers.Visibility = Visibility.Visible;
					tbUsernameInfo.Text = App.User.UserName?.ToString();
					break;
				case "Guest":
				default:
					dgAppointments.Visibility = Visibility.Hidden;
					tciRegisterLogin.Visibility = Visibility.Visible;
					tciGeneral.Visibility = Visibility.Hidden;
					btnLogout.Visibility = Visibility.Hidden;
					tciUsers.Visibility = Visibility.Hidden;
					tbUsernameInfo.Text = string.Empty;
					break;
			}
		}
	}
}