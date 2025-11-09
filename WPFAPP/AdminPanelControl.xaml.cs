using Microsoft.AspNetCore.Identity;
using Models;
using System.Windows.Controls;

namespace WPFAPP {
	public partial class AdminPanelControl : UserControl {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		private readonly RoleControl roleControl;
		private readonly VehicleAssignmentControl vehicleControl;
		private readonly ToDoControl toDoControl;

		public AdminPanelControl(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();

			// Instantiate controls and their containers
			roleControl = new(_context, _userManager);
			vehicleControl = new(_context);
			toDoControl = new(_context);
			tciUsersTab.Content = roleControl;
			tciVehicleAssignmentTab.Content = vehicleControl;
			tciToDosTab.Content = toDoControl;
		}

		private void grDetails_InfoChanged(object sender, SelectionChangedEventArgs e) {
			
		}
	}
}
