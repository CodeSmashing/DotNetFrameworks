using Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace WPFAPP {
	public partial class AppointmentControl : UserControl {
		private readonly AgendaDbContext _context;

		public AppointmentControl(AgendaDbContext context) {
			_context = context;
			InitializeComponent();

			// Subscribe to app user change event
			App.UserChanged += HandleUserChanged;

			// Subscribe to double-click event on DataGrid rows
			dgAppointments.MouseDoubleClick += dgAppointments_MouseDoubleClick;

			// Load appointment types into combobox
			cbTypes.ItemsSource = _context.AppointmentTypes.ToList();

			// Load appointments data
			UpdateDgAppointments();

			// After reloading the items, update the button visibility
			UpdateUIButtons();
		}

		public void HandleUserChanged(object? sender, PropertyChangedEventArgs e) {
			// Ensure button visibility is correct after successful login/registry
			UpdateUIButtons();
		}

		private void dgAppointments_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if (dgAppointments.SelectedItem is Appointment selectedAppointment) {
				// Show appointment details in a popup
				MessageBox.Show(
					 $"To: {selectedAppointment.Date}\n" +
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
			grDetails.Visibility = Visibility.Collapsed;

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

					// Reset the selected appointment and refresh the DataGrid
					dgAppointments.SelectedItem = CollectionView.NewItemPlaceholder;
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

				appointment.AgendaUserId = App.User.Id;
				appointment.Date = contextAppointment.Date;
				appointment.Title = contextAppointment.Title;
				appointment.Description = contextAppointment.Description;
				appointment.AppointmentType = contextAppointment.AppointmentType;

				// Save to database
				_context.Appointments.Add(appointment);
				_context.SaveChanges();
				grDetails.Visibility = Visibility.Collapsed;

				// Select the newly created appointment and refresh the DataGrid
				dgAppointments.SelectedItem = appointment;
				UpdateDgAppointments();
			} catch (Exception ex) {
				MessageBox.Show(
					 $"Error: {ex.Message}\n" +
					"U zult andere waarden moeten proberen of contact opnemen met de support.",
					 "Fout details-scherm afspraak aanmaak",
					 MessageBoxButton.OK,
					 MessageBoxImage.Error
				);
			}
		}

		// Handler to show save button when all details are changed
		private void grDetails_InfoChanged(object sender, EventArgs e) {
			if (dpDate.SelectedDate > DateTime.Now &&
				tbTitle.Text.Length > 0 &&
				tbDescription.Text.Length > 0 &&
				cbTypes.SelectedItem != null) {
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
				dgAppointments.ItemsSource = _context.Appointments
					.Where(app => app.Deleted >= DateTime.Now
						&& app.AgendaUserId == App.User.Id
						&& (tbFilter.Text.Length == 0
							|| (app.Title.Contains(tbFilter.Text)
								|| app.Description.Contains(tbFilter.Text))))
					.OrderBy(app => app.Date)
					.Select(app => app)
					//.Include(app => app.AppointmentType)  // Eager loading van AppointmentType
					.ToList();
			}
		}

		// Refresh the appointments DataGrid with current data from the database
		public void UpdateDgAppointments() {
			dgAppointments.ItemsSource = _context.Appointments
				.Where(app => app.Deleted >= DateTime.Now
					&& !app.IsCompleted
					&& app.AgendaUserId == App.User.Id
					&& App.User.Id != AgendaUser.Dummy.Id)
				.OrderBy(app => app.Date)
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
			if (dgAppointments.SelectedItem == CollectionView.NewItemPlaceholder || dgAppointments.SelectedIndex < 0) {
				// Default states
				btnEdit.IsEnabled = false;
				btnDelete.IsEnabled = false;
				btnSave.IsEnabled = false;
				return;
			}

			// If the selected item is an Appointment, evaluate ownership and validity
			if (dgAppointments.SelectedItem is Appointment selectedAppointment) {
				// If deleted already -> keep hidden/disabled
				if (selectedAppointment.Deleted <= DateTime.Now) {
					return;
				}

				// Only allow edit/delete for appointments belonging to the logged-in user
				if (App.User != null && App.User != AgendaUser.Dummy && selectedAppointment.AgendaUserId == App.User.Id) {
					btnEdit.IsEnabled = true;
					btnDelete.IsEnabled = true;
				} else {
					btnEdit.IsEnabled = false;
					btnDelete.IsEnabled = false;
				}
				btnSave.IsEnabled = false;

				// Update the grDetails grid to use the currently selected appointment data
				grDetails.DataContext = selectedAppointment;
			} else {
				grDetails.DataContext = CollectionView.NewItemPlaceholder;
			}
		}
	}
}
