using Microsoft.AspNetCore.Identity;
using Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFAPP {
	public partial class AppointmentControl : UserControl {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;

		public AppointmentControl(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();

			// Subscribe to app user change event
			App.UserChanged += HandleUserChanged;

			// Subscribe to double-click event on DataGrid rows
			dgAppointments.MouseDoubleClick += dgAppointments_MouseDoubleClick;

			// Load appointment types into combobox
			cbTypes.ItemsSource = _context.AppointmentTypes.ToList();

			// Load appointments data
			UpdateDgAppointments();
		}

		public void HandleUserChanged(object? sender, PropertyChangedEventArgs e) {
			// Ensure button visibility is correct after successful login/registry
			UpdateUIButtons();
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

		private void dgAppointments_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			grDetails.Visibility = Visibility.Hidden;

			// Existing index-based logic removed in favour of clearer helper
			UpdateUIButtons();
		}

		// Add, Edit, Delete, Save button handlers
		private void btnAdd_Click(object sender, RoutedEventArgs e) {
			grDetails.Visibility = Visibility.Visible;
			grDetails.DataContext = new Appointment();
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e) {
			if (btnEdit.Content.ToString() == "Wijzigen") {
				btnEdit.Content = "Opslaan";
			} else if (btnEdit.Content.ToString() == "Opslaan") {
				btnEdit.Content = "Wijzigen";
			}

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
				btnSave.IsEnabled = false;
				dpFrom.SelectedDate = DateTime.Now;
				tbTitle.Text = string.Empty;
				tbDescription.Text = string.Empty;
				grDetails.Visibility = Visibility.Hidden;

				// Refresh appointments DataGrid
				UpdateDgAppointments();
			} catch (Exception erorrInfo) {
				Console.WriteLine("Fout bij opslaan afspraak; " + erorrInfo.Message);
			}
		}

		// Handler to show save button when all details are changed
		private void grDetails_InfoChanged(object sender, EventArgs e) {
			if (dpFrom.SelectedDate > DateTime.Now && tbTitle.Text.Length > 0 && tbDescription.Text.Length > 0) {
				btnSave.IsEnabled = true;
			} else {
				btnSave.IsEnabled = false;
			}
		}

		private void tbFilter_TextChanged(object sender, TextChangedEventArgs e) {
			if (tbFilter.Text != "") {
				tbFilterPlaceholder.Visibility = Visibility.Hidden;
			} else {
				tbFilterPlaceholder.Visibility = Visibility.Visible;
			}
		}

		// Refresh the appointments DataGrid with current data from the database
		public void UpdateDgAppointments() {
			dgAppointments.ItemsSource = _context.Appointments
										.Where(app => app.Deleted >= DateTime.Now
													  && app.From > DateTime.Now
													  && app.UserId == App.User.Id)
										.OrderBy(app => app.From)
										.Select(app => app)
										//.Include(app => app.AppointmentType)  // Eager loading van AppointmentType
										.ToList();

			// After reloading the items, update the button visibility
			UpdateUIButtons();
		}

		// Helper to update button visibility based on selected appointment and user state
		public void UpdateUIButtons() {
			// btnAdd visible when a real user is logged in
			btnAdd.IsEnabled = (App.User != null && App.User != AgendaUser.Dummy);

			// If no selection, nothing more to do
			if (dgAppointments.SelectedItem == null || dgAppointments.SelectedIndex < 0) {
				// Default states
				btnEdit.IsEnabled = false;
				btnDelete.IsEnabled = false;
				btnSave.IsEnabled = false;
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
					btnEdit.IsEnabled = true;
					btnDelete.IsEnabled = true;
				} else {
					btnEdit.IsEnabled = false;
					btnDelete.IsEnabled = false;
				}
				btnSave.IsEnabled = false;

				// Update the grDetails grid to use the currently selected appointment data
				dpFrom.SelectedDate = selectedAppointment.From;
				tbTitle.Text = selectedAppointment.Title;
				tbDescription.Text = selectedAppointment.Description;
			} else {
				dpFrom.SelectedDate = DateTime.Now;
				tbTitle.Text = string.Empty;
				tbDescription.Text = string.Empty;
			}
		}
	}
}
