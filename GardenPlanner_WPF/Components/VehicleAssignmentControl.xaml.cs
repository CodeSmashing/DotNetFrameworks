using Models;
using Models.Enums;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GardenPlanner_WPF {
	public partial class VehicleAssignmentControl : UserControl {
		private readonly AgendaDbContext _context;
		public List<Tuple<Vehicle, ComboBoxItem>> selectedVehicles = new List<Tuple<Vehicle, ComboBoxItem>>();
		private bool _isSettingDataContext = false;
		private bool _isEditing = false;

		private AgendaUser? SelectedEmployee {
			get => (AgendaUser?) cmbEmployees.SelectedItem;
		}
		private Vehicle? SelectedVehicle {
			get => (Vehicle?) cmbVehicles.SelectedItem;
		}

		// Define input requirements
		// Key: Field name, Value: Human-readable name
		public Dictionary<Control, string> inputRequirements = new();

		public VehicleAssignmentControl(AgendaDbContext context) {
			_context = context;
			InitializeComponent();

			// Set the input requirements
			inputRequirements[tbLicencePlate] = "Nummer plaat";
			inputRequirements[cbVehicleType] = "Type voertuig";
			inputRequirements[tbBrand] = "Merk";
			inputRequirements[tbModel] = "Model";
			inputRequirements[tbLoadCapacity] = "Laad capaciteit";
			inputRequirements[tbWeightCapacity] = "Gewicht capaciteit";
			inputRequirements[cbFuelType] = "Brandstof type";

			// Load data into combo boxes
			cbVehicleType.ItemsSource = Enum.GetValues<VehicleType>().Cast<VehicleType>();
			cbFuelType.ItemsSource = Enum.GetValues<FuelType>().Cast<FuelType>();

			// Update grids and visuals
			GetEmployees();
			UpdateDataGrid();
		}

		private void dgVehicleDetails_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			grDetailsInputs.Visibility = Visibility.Collapsed;
			UpdateUIButtons();
		}

		private void dgVehicleDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (dgVehicleDetails.SelectedItem is Vehicle selectedVehicle)
			{
				try
				{
					// Juiste voertuig ophalen + validatie
					Vehicle? vehicle = _context.Vehicles.FirstOrDefault(v => v.Id == selectedVehicle.Id);
					if (vehicle == null)
					{
						MessageBox.Show("Voertuig niet gevonden.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}

					// Ophalen toegewezen gebruikers
					var assignedUsers = _context.Users
						.Where(u => u.VehicleId == vehicle.Id)
						.ToList();
					// Controleren of er gebruikers zijn toegewezen
					if (assignedUsers == null || assignedUsers.Count == 0)
					{
						MessageBox.Show("Geen gebruikers toegewezen aan dit voertuig.", "Voertuig - toegewezen gebruikers", MessageBoxButton.OK, MessageBoxImage.Information);
						return;
					}

					// Tonen van toegewezen gebruikers
					var sb = new StringBuilder();
					foreach (var user in assignedUsers)
					{
						string name = !string.IsNullOrWhiteSpace(user.DisplayName)
							? user.DisplayName
							: $"{user.FirstName} {user.LastName}".Trim();
						sb.AppendLine($"{name} ({user.Email})");
					}

					MessageBox.Show(sb.ToString(), $"Gebruikers toegewezen aan {vehicle.LicencePlate}", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Fout bij ophalen toegewezen gebruikers: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		public void UpdateUIButtons() {
			if (App.User == null) {
				return;
			}

			// btnAdd visible when a real user is logged in
			btnAdd.IsEnabled = (App.User != AgendaUser.Dummy);

			// If no selection, nothing more to do
			if (dgVehicleDetails.SelectedItem == CollectionView.NewItemPlaceholder || dgVehicleDetails.SelectedIndex < 0) {
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
			if (roleList.Count > 0 && (roleList.Contains("Admin") || roleList.Contains("Employee"))) {
				btnEdit.IsEnabled = true;
				btnDelete.IsEnabled = true;
			}
			btnSave.IsEnabled = false;

			// Update the grDetails grid to use the currently selected vehicle data
			SetDataContextWithoutTriggeringEvent(dgVehicleDetails.SelectedItem);
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
			SetDataContextWithoutTriggeringEvent(dgVehicleDetails.SelectedItem);
			btnSave.IsEnabled = false;
			btnEdit.IsEnabled = false;
			btnAdd.IsEnabled = false;
			_isEditing = true;
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e) {
			try {
				Vehicle vehicle = (Vehicle) dgVehicleDetails.SelectedItem;
				Vehicle? contextVehicle = _context.Vehicles.FirstOrDefault(v => v.Id == vehicle.Id);

				if (contextVehicle != null) {
					contextVehicle.Deleted = DateTime.Now;
					_context.SaveChanges();

					// Reset the selected Vehicle and refresh the DataGrid
					dgVehicleDetails.SelectedItem = CollectionView.NewItemPlaceholder;
					UpdateDataGrid();
				}
			} catch (Exception errorInfo) {
				Console.WriteLine("Fout bij verwijderen voertuig; " + errorInfo.Message);
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
				Vehicle vehicle;
				if (_isEditing) {
					// Attempt to update the vehicle info
					vehicle = _context.Vehicles.First(app => app.Id == ((Vehicle) grDetailsInputs.DataContext).Id);

					vehicle.LicencePlate = tbLicencePlate.Text;
					vehicle.VehicleType = (VehicleType) cbVehicleType.SelectedItem;
					vehicle.Brand = tbBrand.Text;
					vehicle.Model = tbModel.Text;
					vehicle.LoadCapacity = double.Parse(tbLoadCapacity.Text);
					vehicle.WeightCapacity = double.Parse(tbWeightCapacity.Text);
					vehicle.FuelType = (FuelType) cbFuelType.SelectedItem;
					_context.Vehicles.Update(vehicle);
					_isEditing = false;
				} else {
					// Attempt to create a new vehicle instance
					vehicle = new() {
						LicencePlate = tbLicencePlate.Text,
						VehicleType = (VehicleType) cbVehicleType.SelectedItem,
						Brand = tbBrand.Text,
						Model = tbModel.Text,
						LoadCapacity = double.Parse(tbLoadCapacity.Text),
						WeightCapacity = double.Parse(tbWeightCapacity.Text),
						FuelType = (FuelType) cbFuelType.SelectedItem,
					};

					// Save to database
					_context.Vehicles.Add(vehicle);
				}
				_context.SaveChanges();
				grDetailsInputs.Visibility = Visibility.Collapsed;

				// Show success message
				MessageBox.Show("Voertuig succesvol aangemaakt.");

				// Select the vehicle and refresh the DataGrid
				UpdateDataGrid();
				dgVehicleDetails.SelectedItem = vehicle;
			} catch (Exception ex) {
				MessageBox.Show(
					 $"Error: {ex.Message}\n" +
					"U zult andere waarden moeten proberen of contact opnemen met de support.",
					 "Fout details-scherm voertuig aanmaak",
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

		public void UpdateDataGrid() {
			dgVehicleDetails.ItemsSource = _context.Vehicles
				.Where(v => v.Deleted >= DateTime.Now)
				.OrderBy(v => v.VehicleType)
				.ToList();

			UpdateVehicleComboBox();
			// After reloading the items, update the button visibility
			UpdateUIButtons();
		}

		public void btnAssign_Click(object sender, RoutedEventArgs e) {
			if (SelectedVehicle is Vehicle selectedVehicle && SelectedEmployee is AgendaUser selectedEmployee) {
				Vehicle? contextVehicle = _context.Vehicles.FirstOrDefault(v => v.Id == selectedVehicle.Id);
				AgendaUser? contextEmployee = _context.Users.FirstOrDefault(u => u.Id == selectedEmployee.Id);
				if (contextVehicle != null && contextEmployee != null) {
					contextVehicle.IsInUse = true;
					contextEmployee.VehicleId = contextVehicle.Id;
					_context.SaveChanges();
					UpdateDataGrid();
					GetEmployees();
				}
			}
		}

		public void cmbVehicles_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			imgVehicle.Source = SelectedVehicle is Vehicle selectedVehicle && selectedVehicle.ImageUrl != null
				? new BitmapImage(new Uri(selectedVehicle.ImageUrl))
				: null;
			if(SelectedVehicle != null && SelectedEmployee != null){
				btnAssign.IsEnabled = true;
			}
		}

		public void cmbEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (SelectedEmployee != null) {
				if (SelectedVehicle != null) {
					btnAssign.IsEnabled = true;
				}

				lblCurentVehicle.Content = _context.Vehicles
					.Where(v => v.Id == SelectedEmployee.VehicleId)
					.Select(v => v.LicencePlate)
					.FirstOrDefault() ?? "Geen voertuig toegewezen";
			}
		}

		public void UpdateVehicleComboBox() {
			cmbVehicles.ItemsSource = _context.Vehicles
				.Where(v => v.Deleted > DateTime.Now)
				.ToList();

			//List<Vehicle> vehicles = _context.Vehicles.Where(v => v.Deleted > DateTime.Now).ToList();

			//foreach (var vehicle in vehicles) {
			//	Image newimg = new Image();
			//	newimg.Source = new BitmapImage(new Uri(vehicle.ImageUrl));
			//	cmbVehicles.Items.Add(new ComboBoxItem { Content = newimg, Width = 100, Height = 100 });
			//	selectedVehicles.Add(new Tuple<Vehicle, ComboBoxItem>(vehicle, (ComboBoxItem) cmbVehicles.Items[cmbVehicles.Items.Count - 1]));
			//}
		}

		public void GetEmployees() {
			cmbEmployees.ItemsSource = _context.UserRoles
				.Where(u => u.RoleId == "Employee")
				.Select(ur => ur.UserId)
				.Join(_context.Users,
					userRoleId => userRoleId,
					user => user.Id,
					(userRoleId, user) => user)
				.ToList();
		}

		private void SetDataContextWithoutTriggeringEvent(object item) {
			_isSettingDataContext = true;
			grDetailsInputs.DataContext = item;
			_isSettingDataContext = false;
		}
	}
}
