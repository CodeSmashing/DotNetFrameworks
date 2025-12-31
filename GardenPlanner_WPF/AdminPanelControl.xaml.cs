using Microsoft.AspNetCore.Identity;
using Models;
using System.Windows.Controls;

namespace GardenPlanner_WPF {
	public partial class AdminPanelControl : UserControl {
		private readonly GlobalDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		private readonly AgendaUserControl agendaUserControl;
		private readonly RoleControl roleControl;
		private readonly VehicleAssignmentControl vehicleControl;
		private readonly ToDoControl toDoControl;

		public AdminPanelControl(GlobalDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();

			// Instantiate controls and their containers
			agendaUserControl = new(_context, _userManager);
			roleControl = new(_context, _userManager);
			vehicleControl = new(_context);
			toDoControl = new(_context);
			tciUsersTab.Content = agendaUserControl;
			tciRolesTab.Content = roleControl;
			tciVehicleAssignmentTab.Content = vehicleControl;
			tciToDosTab.Content = toDoControl;
		}
	}
}
