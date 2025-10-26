using Microsoft.AspNetCore.Identity;
using Models;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace WPFAPP {
	public partial class RoleControl : UserControl {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		private bool _isUnselecting = false;

		private AgendaUser? User {
			get => (AgendaUser) cbUsers.SelectedItem;
		}

		public RoleControl(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();

			cbUsers.ItemsSource = _context.Users
				.Where(u => u.LockoutEnd == null || u.LockoutEnd < DateTime.Now)
				.OrderBy(u => u.LastName + " " + u.FirstName)
				.ToList();

			// Explicitly create ListBoxItems instead of setting ItemSource directly to allow for setting their style
			foreach (var role in _context.Roles) {
				lbRoles.Items.Add(new ListBoxItem { Content = role.Id });
			}
		}

		public void cbUsers_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (User != AgendaUser.Dummy || User != null) {
				UpdateListBoxRoles();
			}
		}

		public async void lbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (_isUnselecting) {
				return;
			}

			if (User == AgendaUser.Dummy || User == null) {
				return;
			}

			var affectedItems = e.AddedItems ?? e.RemovedItems;

			if (affectedItems.Count > 0) {
				string? role = ((ListBoxItem?) affectedItems[0])?.Content.ToString();

				if (role != null) {
					if (await _userManager.IsInRoleAsync(User, role)) {
						await _userManager.RemoveFromRoleAsync(User, role);
					} else {
						await _userManager.AddToRoleAsync(User, role);
					}
				}
			}

			UpdateListBoxRoles();
			UnselectAllWithoutTriggeringEvent();
		}

		private void UnselectAllWithoutTriggeringEvent() {
			_isUnselecting = true;
			lbRoles.UnselectAll();
			_isUnselecting = false;
		}

		public List<string> GetUserRoles(AgendaUser user) {
			return _context.UserRoles
				.Where(ur => ur.UserId == user.Id)
				.Select(ur => ur.RoleId)
				.ToList();
		}

		// Update ListBoxItem styles
		private void UpdateListBoxRoles() {
			foreach (ListBoxItem item in lbRoles.Items) {
				string? role = item.Content.ToString();

				if (role != null && User != null) {
					if (GetUserRoles(User).Contains(role)) {
						item.Style = (Style) FindResource("CheckedListBoxItem");
					} else {
						item.Style = null;
					}
				}
			}
		}
	}
}
