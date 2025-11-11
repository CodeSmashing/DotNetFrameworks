using Models;
using System.Windows;
using System.Windows.Controls;
using WPFAPP.Components;
using WPFAPP.CustomEventArgs;

namespace WPFAPP {
	public partial class ToDoControl : UserControl {
		private readonly AgendaDbContext _context;

		public ToDoControl(AgendaDbContext context) {
			_context = context;
			InitializeComponent();

			// Subscribe to events
			AppointmentControl.AppointmentCreated += UpdateDataGrid;
			//DetailsControl.ItemCreated += UpdateDataGrid;
			UpdateDataGrid();
		}

		public void UpdateDataGrid(object? sender = null, EventArgs? e = null) {
			//if (e != null && e is not ItemCreatedEventArgs) {
			//	return;
			//}

			dgAppointments.ItemsSource = _context.Appointments
				.Where(v => v.Deleted > DateTime.Now)
				.ToList();
		}

		public void btnAssign_Click(object sender, System.Windows.RoutedEventArgs e) {
			ToDo todo;
			if (tbContent.Text.Trim() == "") {
				MessageBox.Show("ToDo description cannot be empty.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			try {
				todo = new() {
					Description = tbContent.Text,
					AppointmentId = (dgAppointments.SelectedItem as Appointment)!.Id
				};
			} catch {
				MessageBox.Show("Please select an appointment and enter a valid ToDo description.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			// Save to database
			_context.ToDos.Add(todo);
			_context.SaveChanges();

			// Refresh DataGrid
			UpdateDataGrid();
			// Refresh ToDo DataGrid
			dgAppointments_SelectionChanged();
			// Clear input fields
			tbContent.Text = string.Empty;
		}

		public void dgAppointments_SelectionChanged(object? sender = null, SelectionChangedEventArgs? e = null) {
			if (dgAppointments.SelectedItem is Appointment selectedAppointment) {
				dgTodos.ItemsSource = _context.ToDos
					.Where(t => t.Deleted >= DateTime.Now && t.AppointmentId == selectedAppointment.Id)
					.OrderBy(t => t.Id)
					.ToList();
			}
		}

		public void btnRefresh_Click(object sender, System.Windows.RoutedEventArgs e) {
			UpdateDataGrid();
		}

		public void btnFinish_Click(object sender, System.Windows.RoutedEventArgs e) {
			if (dgTodos.SelectedItem is ToDo selectedToDo) {
				selectedToDo.Ready = true;
				// Save to database
				_context.ToDos.Update(selectedToDo);
				_context.SaveChanges();
				// Refresh DataGrid
				UpdateDataGrid();
				// Refresh ToDo DataGrid
				dgAppointments_SelectionChanged();
			}
		}

		public void btnUnfinish_Click(object sender, System.Windows.RoutedEventArgs e) {
			if (dgTodos.SelectedItem is ToDo selectedToDo) {
				selectedToDo.Ready = false;
				// Save to database
				_context.ToDos.Update(selectedToDo);
				_context.SaveChanges();
				// Refresh DataGrid
				UpdateDataGrid();
				// Refresh ToDo DataGrid
				dgAppointments_SelectionChanged();
			}
		}
	}
}
