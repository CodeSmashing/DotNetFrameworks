using Microsoft.AspNetCore.Identity;
using Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPFAPP {
	public partial class AdminPanelControl : UserControl {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		public List<Tuple<Vehicle, ComboBoxItem>> selectedVehicles = new List<Tuple<Vehicle, ComboBoxItem>>();
		private readonly RoleControl roleControl;

		public AdminPanelControl(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();

			// Instantiate controls and their containers
			roleControl = new(_context, _userManager);
			UsersContainer.Children.Clear();
			UsersContainer.Children.Add(roleControl);

			GetEmployees();

			List<Vehicle> vehicles = _context.Vehicles.ToList();

			foreach (var vehicle in vehicles) {
				Image newimg = new Image();
				newimg.Source = new BitmapImage(new Uri(vehicle.ImageUrl));
				cmbVehicles.Items.Add(new ComboBoxItem { Content = newimg, Width = 200 });
				selectedVehicles.Add(new Tuple<Vehicle, ComboBoxItem>(vehicle, (ComboBoxItem) cmbVehicles.Items[cmbVehicles.Items.Count - 1]));
			}
		}

		private void grVehicleDetails_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			// Ensure an item is selected and is a ComboBoxItem
			ComboBoxItem selectedItem = cmbVehicles.SelectedItem as ComboBoxItem;
			if (selectedItem == null) {
				grVehicleDetails.Visibility = Visibility.Collapsed;
				grVehicleDetails.DataContext = null;
				return;
			}

			// Find the vehicle tuple that has the same ComboBoxItem (compare by reference)
			var match = selectedVehicles.FirstOrDefault(t => ReferenceEquals(t.Item2, selectedItem));

			if (match != null) {
				// Show details and bind to the vehicle
				grVehicleDetails.Visibility = Visibility.Visible;
				grVehicleDetails.DataContext = match.Item1;

			} else {
				// No match found: hide/clear details
				grVehicleDetails.Visibility = Visibility.Collapsed;
				grVehicleDetails.DataContext = null;
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
