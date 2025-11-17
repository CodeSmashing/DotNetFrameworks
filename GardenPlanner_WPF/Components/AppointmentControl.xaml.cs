using Models;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace GardenPlanner_WPF {
	public partial class AppointmentControl : UserControl {
		static public event EventHandler AppointmentCreated = delegate { };
		private readonly AgendaDbContext _context;
		private bool _isSettingDataContext = false;
		private bool _isEditing = false;

		// Define input requirements
		// Key: Field name, Value: Human-readable name
		public Dictionary<Control, string> inputRequirements = new();

		public AppointmentControl(AgendaDbContext context) {
			_context = context;
			InitializeComponent();

			// Set the input requirements
			inputRequirements[dpDate] = "Datum";
			inputRequirements[tbTitle] = "Titel";
			inputRequirements[cbTypes] = "Type Afspraak";
			inputRequirements[tbDescription] = "Beschrijving";

			// Subscribe to events
			App.UserChanged += HandleUserChanged;
			dgAppointments.MouseDoubleClick += dgAppointments_MouseDoubleClick;

			// Load data into combo boxes
			cbTypes.ItemsSource = _context.AppointmentTypes.ToList();

			// Update grids and visuals
			UpdateDataGrid();
			UpdateUIButtons();
		}

		public void HandleUserChanged(object? sender, PropertyChangedEventArgs e) {
			// Ensure button visibility is correct after successful login/registry
			UpdateUIButtons();
		}

		private void dgAppointments_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if (dgAppointments.SelectedItem is Appointment selectedAppointment) {
				// Build a details string including related ToDos
				var todos = _context.ToDos
					.Where(t => t.Deleted >= DateTime.Now && t.AppointmentId == selectedAppointment.Id)
					.OrderBy(t => t.Id)
					.ToList();

				string todosText = todos.Count > 0 ? string.Join("\n", todos) : "(Geen todos)";

				string message =
					$"Titel: {selectedAppointment.Title}\n" +
					$"Datum: {selectedAppointment.Date}\n\n" +
					$"Beschrijving:\n{selectedAppointment.Description}\n\n" +
					$"Todos:\n{todosText}";

				MessageBox.Show(
					message,
					"Afspraak details",
					MessageBoxButton.OK,
					MessageBoxImage.Information
				);
			}
		}

		private void dgAppointments_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			grDetailsInputs.Visibility = Visibility.Collapsed;
			UpdateUIButtons();
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e) {
			grDetailsInputs.Visibility = Visibility.Visible;
			SetDataContextWithoutTriggeringEvent(CollectionView.NewItemPlaceholder);
			btnSave.IsEnabled = false;
			btnEdit.IsEnabled = false;
			_isEditing = false;
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e) {
			grDetailsInputs.Visibility = Visibility.Visible;
			SetDataContextWithoutTriggeringEvent(dgAppointments.SelectedItem);
			btnSave.IsEnabled = false;
			btnEdit.IsEnabled = false;
			btnAdd.IsEnabled = false;
			_isEditing = true;
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
					UpdateDataGrid();
				}
			} catch (Exception errorInfo) {
				Console.WriteLine("Fout bij verwijderen afspraak; " + errorInfo.Message);
				UpdateDataGrid();
			}
		}

		private void btnSave_Click(object sender, RoutedEventArgs e) {
			// Clear previous errors
			spError.Children.Clear();

			// Validate inputs and show errors
			MainWindow.ValidateInputs(out List<Control> errors, in inputRequirements);

			foreach (Control field in inputRequirements.Keys) {
				field.ClearValue(Border.BorderBrushProperty);

				// If invalid, add error
				if (errors.Contains(field)) {
					field.BorderBrush = Brushes.Red;
					spError.Children.Add(new TextBlock { Text = $"{inputRequirements[field]} is vereist" });
				}
			}

			// If there are errors, return early
			if (spError.Children.Count > 0) {
				return;
			}

			try {
				Appointment appointment;
				if (_isEditing) {
					// Attempt to update the appointment info
					appointment = _context.Appointments.First(app => app.Id == ((Appointment) grDetailsInputs.DataContext).Id);

					appointment.Date = dpDate.SelectedDate ?? DateTime.MinValue;
					appointment.Title = tbTitle.Text;
					appointment.AppointmentType = (AppointmentType) cbTypes.SelectedItem;
					appointment.Description = tbDescription.Text;

					// Save to database
					_context.Appointments.Update(appointment);
					_context.SaveChanges();
					_isEditing = false;
				} else {
					// Attempt to create a new appointment instance
					appointment = new() {
						AgendaUserId = App.User.Id,
						Date = dpDate.SelectedDate ?? DateTime.MinValue,
						Title = tbTitle.Text,
						AppointmentTypeId = ((AppointmentType) cbTypes.SelectedItem).Id,
						Description = tbDescription.Text,
					};

					// Save to database
					_context.Appointments.Add(appointment);
					_context.SaveChanges();

					AppointmentCreated?.Invoke(typeof(AppointmentControl), new EventArgs());
				}
				grDetailsInputs.Visibility = Visibility.Collapsed;

				// Show success message
				MessageBox.Show("Afspraak succesvol aangemaakt.");

				// Select the appointment and refresh the DataGrid
				UpdateDataGrid();
				dgAppointments.SelectedItem = appointment;

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
			if (_isSettingDataContext) {
				return;
			}
			btnSave.IsEnabled = MainWindow.ValidateInputs(out _, in inputRequirements);
		}

		private void tbFilter_TextChanged(object sender, TextChangedEventArgs e) {
			if (tbFilter.Text != "") {
				tbFilterPlaceholder.Visibility = Visibility.Hidden;
				dgAppointments.ItemsSource = _context.Appointments
					.Where(app => app.Deleted >= DateTime.Now
						&& app.AgendaUserId == App.User.Id
						&& (tbFilter.Text.Length == 0
							|| (app.Title.Contains(tbFilter.Text)
								|| app.Description.Contains(tbFilter.Text))))
					.OrderBy(app => app.Date)
					.Select(app => app)
					.ToList();
			} else {
				tbFilterPlaceholder.Visibility = Visibility.Visible;
				UpdateDataGrid();
			}
		}

		// Refresh the appointments DataGrid with current data from the database
		public void UpdateDataGrid() {
			dgAppointments.ItemsSource = _context.Appointments
				.Where(app =>
					// Non-deleted appointments that are completed and owned by the user
					app.Deleted >= DateTime.Now
						&& !app.IsCompleted
							&& app.AgendaUserId == App.User.Id)
				.OrderBy(app => app.Date)
				.ToList();

			UpdateUIButtons();
		}

		// Helper to update button visibility based on selected appointment and user state
		public void UpdateUIButtons() {
			if (App.User == null) {
				return;
			}

			// btnAdd visible when a real user is logged in
			btnAdd.IsEnabled = (App.User != AgendaUser.Dummy);

			// If no selection, nothing more to do
			if (dgAppointments.SelectedItem == CollectionView.NewItemPlaceholder || dgAppointments.SelectedIndex < 0) {
				// Default states
				btnEdit.IsEnabled = false;
				btnDelete.IsEnabled = false;
				btnSave.IsEnabled = false;
				SetDataContextWithoutTriggeringEvent(CollectionView.NewItemPlaceholder);

				return;
			}

			// Only allow edit/delete to the logged-in user if they have the correct permissions
			List<string> roleList = _context.UserRoles
				.Where(ur => ur.UserId == App.User.Id)	
				.Select(ur => ur.RoleId)
				.ToList();
			if (roleList.Count > 0 && (roleList.Contains("User") || roleList.Contains("UserAdmin"))) {
				btnEdit.IsEnabled = true;
				btnDelete.IsEnabled = true;
			}
			btnSave.IsEnabled = false;

			// Update the grDetails grid to use the currently selected appointment data
			SetDataContextWithoutTriggeringEvent(dgAppointments.SelectedItem);
		}

		private void SetDataContextWithoutTriggeringEvent(object item) {
			_isSettingDataContext = true;
			grDetailsInputs.DataContext = item;
			_isSettingDataContext = false;
		}
	}
}
