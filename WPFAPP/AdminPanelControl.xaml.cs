using Microsoft.AspNetCore.Identity;
using Models;
using System.Windows.Controls;

namespace WPFAPP {
	public partial class AdminPanelControl : UserControl {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		private readonly RoleControl roleControl;
		private readonly VehicleAssignmentControl vehicleControl;

		public AdminPanelControl(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();

			// Instantiate controls and their containers
			roleControl = new(_context, _userManager);
			vehicleControl = new(_context, _userManager);
			UsersContainer.Children.Clear();
			UsersContainer.Children.Add(roleControl);
			VehicleAssignmentContainer.Children.Clear();
			VehicleAssignmentContainer.Children.Add(vehicleControl);
		}

		private void grDetails_InfoChanged(object sender, SelectionChangedEventArgs e) {
			
		}
	}
}
