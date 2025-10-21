using Microsoft.AspNetCore.Identity;
using Models;
using System.Windows;
using System.Windows.Controls;


namespace WPFAPP {
	public partial class RoleControl : UserControl {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		private AgendaUser user = AgendaUser.Dummy;

		public RoleControl(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();

			cbUsers.ItemsSource = _context.Users
											.Where(u => u.LockoutEnd == null || u.LockoutEnd < DateTime.Now)
											.OrderBy(u => u.LastName + " " + u.FirstName)
											.ToList();
		}

		public void cbUsers_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			user = _context.Users.Find(((AgendaUser) cbUsers.SelectedItem).Id) ?? AgendaUser.Dummy;
			if (user == AgendaUser.Dummy) {
				lbRoles.Visibility = Visibility.Hidden;
				return;
			} else {
				List<ListBoxItem> roles = new();
				List<string> userRoles = _context.UserRoles
													.Where(ur => ur.UserId == user.Id)
													.Select(ur => ur.RoleId)
													.ToList();

				// Populate roles ListBox
				foreach (IdentityRole role in _context.Roles) {
					bool isSelected = userRoles.Contains(role.Id);
					roles.Add(new() {
						Content = role.Id,
						IsSelected = isSelected
					});
				}
				lbRoles.ItemsSource = roles;
				lbRoles.Visibility = Visibility.Visible;
			}
		}

		public async void lbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (user == AgendaUser.Dummy) {
				return;
			}

			// Update roles
			foreach (ListBoxItem item in lbRoles.Items) {
				string role = item.Content.ToString() ?? string.Empty;

				if (lbRoles.SelectedItems.Contains(item)) {
					await _userManager.AddToRoleAsync(user, role);
				} else {
					await _userManager.RemoveFromRoleAsync(user, role);
				}
			}
		}
	}
}
