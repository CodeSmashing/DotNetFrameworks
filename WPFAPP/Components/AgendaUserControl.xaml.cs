using Microsoft.AspNetCore.Identity;
using Models;
using System.Windows.Controls;

namespace WPFAPP {
	public partial class AgendaUserControl : UserControl {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		private bool _isSettingDataContext = false;
		private bool _isEditing = false;

		private AgendaUser User {
			get => (AgendaUser) dgUsers.SelectedItem;
		}

		// Define input requirements
		// Key: Field name, Value: Human-readable name
		public Dictionary<Control, string> inputRequirements = new();

		public AgendaUserControl(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();
		}
	}
}
