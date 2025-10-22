using Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WPFAPP {
	public partial class UserInfoControl : UserControl {
		public UserInfoControl() {
			InitializeComponent();

			// Subscribe to app user change event
			App.UserChanged += HandleUserChanged;
		}

		public void HandleUserChanged(object? sender, PropertyChangedEventArgs e) {
			UpdateUIInfo(e.PropertyName);
		}

		private void btnLogout_Click(object sender, RoutedEventArgs e) {
			// Show/hide relevant elements
			App.User = AgendaUser.Dummy;

			// Ensure buttons are updated for logged out state
			//App.MainWindow.UpdateButtonsVisibilityForSelectedRow();

			// Return to authentication tab
			App.MainWindow.tcNavigation.SelectedItem = App.MainWindow.tciAuthenticationTab;
		}

		public void UpdateUIInfo(string? role = "") {
			switch (role) {
				case "User":
				case "Admin":
				case "Useradmin":
				case "Employee":
					tbUsernameInfo.Text = App.User.UserName?.ToString();
					break;
				case "Guest":
				default:
					tbUsernameInfo.Text = string.Empty;
					break;
			}
		}
	}
}
