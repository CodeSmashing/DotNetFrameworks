using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFAPP {
	public partial class VehicleAssignmentControl : UserControl {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		public List<Tuple<Vehicle, ComboBoxItem>> selectedVehicles = new List<Tuple<Vehicle, ComboBoxItem>>();

		public VehicleAssignmentControl(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();

			cbVehicleTypes.ItemsSource = Enum.GetValues(typeof(VehicleType)).Cast<VehicleType>();
			cbFuelTypes.ItemsSource = Enum.GetValues(typeof(FuelType)).Cast<FuelType>();

			GetEmployees();

			UpdateDgVehicles();
		}
		private void grVehicleDetails_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			// Existing index-based logic removed in favour of clearer helper
			UpdateUIButtons();
		}
		public void UpdateUIButtons() {
			btnAdd.IsEnabled = true;

			// If no selection, nothing more to do
			if (grVehicleDetails.SelectedItem == CollectionView.NewItemPlaceholder || grVehicleDetails.SelectedIndex < 0) {
				// Default states
				btnEdit.IsEnabled = false;
				btnDelete.IsEnabled = false;
				btnSave.IsEnabled = false;
				return;
			}

			if (App.User != null && App.User != AgendaUser.Dummy) {
				btnEdit.IsEnabled = true;
				btnDelete.IsEnabled = true;
			}
			btnSave.IsEnabled = false;

			// If the selected item is a vehicle
			if (grVehicleDetails.SelectedItem is Vehicle selectedVehicle) {
				// Update the grDetails grid to use the currently selected vehicle data
				grDetails.DataContext = selectedVehicle;
			} else {
				grDetails.DataContext = CollectionView.NewItemPlaceholder;
			}
		}
		private void btnAdd_Click(object sender, RoutedEventArgs e) {
			grDetails.Visibility = Visibility.Visible;
			grDetails.DataContext = new Vehicle();
			btnSave.IsEnabled = true;
			btnEdit.IsEnabled = false;
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e) {
			grDetails.Visibility = Visibility.Visible;
			grDetails.DataContext = grVehicleDetails.SelectedItem;
			btnSave.IsEnabled = false;
			btnEdit.IsEnabled = false;
			btnAdd.IsEnabled = false;
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e) {
			try {
				Vehicle vehicle = (Vehicle) grVehicleDetails.SelectedItem;
				Vehicle? contextVehicle = _context.Vehicles.FirstOrDefault(v => v.Id == vehicle.Id);

				if (contextVehicle != null) {
					contextVehicle.Deleted = DateTime.Now;
					_context.SaveChanges();
					// Reset the selected Vehicle and refresh the DataGrid
					grVehicleDetails.SelectedItem = CollectionView.NewItemPlaceholder;
					UpdateDgVehicles();
				}
				UpdateDgVehicles();
			} catch (Exception errorInfo) {
				Console.WriteLine("Fout bij verwijderen voertuig; " + errorInfo.Message);
				UpdateDgVehicles();
			}
		}

		private void btnSave_Click(object sender, RoutedEventArgs e) {
			try {
				Vehicle vehicle = new();
				Vehicle contextvehicle = (Vehicle) grDetails.DataContext;

				vehicle.Model = contextvehicle.Model;
				vehicle.Brand = contextvehicle.Brand;
				vehicle.FuelType = contextvehicle.FuelType;
				vehicle.WeightCapacity = contextvehicle.WeightCapacity;
				vehicle.LoadCapacity = contextvehicle.LoadCapacity;
				vehicle.IsManuel = contextvehicle.IsManuel;
				vehicle.LicencePlate = contextvehicle.LicencePlate;
				vehicle.VehicleType = contextvehicle.VehicleType;
				vehicle.Deleted = DateTime.MaxValue;
				vehicle.ImageUrl = "https://teja9.kuikr.com/images/car/default-cars.jpeg";
				vehicle.IsInUse = false;
				vehicle.EmployeeId = 0;



				// Save to database
				_context.Vehicles.Add(vehicle);
				_context.SaveChanges();
				grDetails.Visibility = Visibility.Hidden;

				// Select the newly created vehicle and refresh the DataGrid
				grVehicleDetails.SelectedItem = vehicle;
				UpdateDgVehicles();
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
			if (tbBrand.Text != null &&
				tbModel.Text != null &&
				tbLicencePlate != null &&
				tbLoadCapacity != null &&
				tbWeightCapacity != null) {
				btnEdit.IsEnabled = true;
			} else {
				btnEdit.IsEnabled = false;
			}
		}

		public void UpdateDgVehicles() {
			grVehicleDetails.ItemsSource = _context.Vehicles
				.Where(v => v.Deleted > DateTime.Now)
				.OrderBy(v => v.VehicleType)
				.ToList();

			UpdateVehicleImages();
			// After reloading the items, update the button visibility
			UpdateUIButtons();
		}

		public void UpdateVehicleImages() {
			List<Vehicle> vehicles = _context.Vehicles.Where(v => v.Deleted > DateTime.Now).ToList();

			foreach (var vehicle in vehicles) {
				Image newimg = new Image();
				newimg.Source = new BitmapImage(new Uri(vehicle.ImageUrl));
				cmbVehicles.Items.Add(new ComboBoxItem { Content = newimg, Width = 100, Height = 100 });
				selectedVehicles.Add(new Tuple<Vehicle, ComboBoxItem>(vehicle, (ComboBoxItem) cmbVehicles.Items[cmbVehicles.Items.Count - 1]));
			}
		}

		public async void GetEmployees() {
			cmbEmployees.ItemsSource = _context.UserRoles
									.Where(u => u.RoleId == "Employee")
									.Select(ur => ur.UserId)
									.Join(_context.Users,
											userRoleId => userRoleId,
											user => user.Id,
											(userRoleId, user) => user)
											.Where(user => user.VehicleId == null)
									.ToList();
		}
	}
}
