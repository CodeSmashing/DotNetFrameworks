using Microsoft.AspNetCore.Identity;
using Models;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFAPP.Components {
	public partial class AppointmentControl : UserControl {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		private readonly DetailsControl _detailsControl;
		private readonly IdentityRole<string>[] _permissiveRoles;
		private readonly bool _isSetting = true;

		public AppointmentControl(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			InitializeComponent();
			_context = context;
			_userManager = userManager;
			_permissiveRoles = [
				_context.Roles.First(r => r.Name == "User"),
				_context.Roles.First(r => r.Name == "UserAdmin"),
				_context.Roles.First(r => r.Name == "Admin"),
				_context.Roles.First(r => r.Name == "Employee"),
			];
			_detailsControl = new(_context, _userManager, dgAppointments, typeof(Appointment), _permissiveRoles);

			// Set the input requirements
			_detailsControl.inputRequirements[dpDate] = "Datum";
			_detailsControl.inputRequirements[tbTitle] = "Titel";
			_detailsControl.inputRequirements[cbAppointmentType] = "Type Afspraak";
			_detailsControl.inputRequirements[tbDescription] = "Beschrijving";

			// Set the details control contents
			MainWindow.ReParentElementsTo(
				[lbDate, dpDate, lbTitle, tbTitle, lbAppointmentType, cbAppointmentType, lbDescription, tbDescription],
				_detailsControl.grDetailsInputs
			);
			grAppointmentControl.Children.Remove(spTemporaryContainer);

			// Set rows and columns
			Grid.SetRow(_detailsControl, 4);
			Grid.SetRow(
				(StackPanel) _detailsControl.grDetailsInputs.FindName("spError"),
				_detailsControl.grDetailsInputs.RowDefinitions.Count
			);

			// Set filter options
			_detailsControl.filterOptions.AddRange(
				new(
					cbFilterDeleted.Name,
					[cbFilterDeleted.IsChecked ?? false],
					(app => app.Deleted >= DateTime.Now)
				),
				new(
					cbFilterCompleted.Name,
					[cbFilterCompleted.IsChecked ?? false],
					(app => app.IsCompleted)
				),
				new(
					cbFilterCurrentUser.Name,
					[cbFilterCurrentUser.IsChecked ?? false],
					(app => app.AgendaUserId == App.User.Id)
				),
				new(
					cbFilterMatching.Name,
					[cbFilterMatching.IsChecked ?? false],
					(app => (app.Title.Contains(tbFilter.Text) || app.Description.Contains(tbFilter.Text)))
				)
			);

			// Subscribe to events
			App.UserChanged += HandleUserChanged;
			dpDate.SelectedDateChanged += _detailsControl.grDetails_InfoChanged;
			tbTitle.TextChanged += _detailsControl.grDetails_InfoChanged;
			cbAppointmentType.SelectionChanged += _detailsControl.grDetails_InfoChanged;
			tbDescription.TextChanged += _detailsControl.grDetails_InfoChanged;

			// Load data into combo boxes
			cbAppointmentType.ItemsSource = _context.AppointmentTypes.ToList();

			// Update grids and visuals
			grAppointmentControl.Children.Add(_detailsControl);
			_detailsControl.UpdateDataGrid();
			_detailsControl.UpdateUIButtons();
			_isSetting = false;
		}

		public void HandleUserChanged(object? sender, PropertyChangedEventArgs e) {
			// Ensure button visibility is correct after successful login/registry
			_detailsControl.UpdateUIButtons();
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
			_detailsControl.grDetailsInputs.Visibility = Visibility.Collapsed;
			_detailsControl.UpdateUIButtons();
		}

		private void tbFilter_TextChanged(object sender, TextChangedEventArgs e) {
			if (tbFilter.Text != "") {
				tbFilterPlaceholder.Visibility = Visibility.Hidden;
			} else {
				tbFilterPlaceholder.Visibility = Visibility.Visible;
			}
			_detailsControl.UpdateDataGrid();
		}

		private void FilterOption_Checked(object sender, RoutedEventArgs e) {
			if (_isSetting) {
				return;
			}

			if (sender is CheckBox checkbox) {
				var toggle = _detailsControl.filterOptions
					.First(fo => fo.Key == checkbox.Name)
					.Toggle;

				toggle[0] = checkbox.IsChecked ?? false;
				_detailsControl.UpdateDataGrid();
			}
		}
	}
}
