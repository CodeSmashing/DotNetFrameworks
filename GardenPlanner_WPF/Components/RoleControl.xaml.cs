using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GardenPlanner_WPF {
	public partial class RoleControl : UserControl {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		private bool _isUnselecting = false;

		private AgendaUser? User {
			get => (AgendaUser) dgUsers.SelectedItem;
		}

		public RoleControl(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();

			// Explicitly create ListBoxItems instead of setting ItemSource directly to allow for setting their style
			foreach (var role in _context.Roles) {
				lbRoles.Items.Add(new ListBoxItem { Content = role.Id });
			}

			UpdateDataGrid();
		}

		public void dgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (User != AgendaUser.Dummy || User != null) {
				UpdateUIVisuals();
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

			UpdateUIVisuals();
			UnselectAllWithoutTriggeringEvent();
		}

		private void tbFilter_TextChanged(object sender, TextChangedEventArgs e) {
			// Display or hide the placeholder text
			if (tbFilterUser.Text.Equals(string.Empty)) {
				tbFilterUserPlaceholder.Visibility = Visibility.Visible;
				UpdateDataGrid();
			} else {
				tbFilterUserPlaceholder.Visibility = Visibility.Hidden;
			}

			// Display or hide the placeholder text
			if (tbFilterRole.Text.Equals(string.Empty)) {
				tbFilterRolePlaceholder.Visibility = Visibility.Visible;
				UpdateDataGrid();
			} else {
				tbFilterRolePlaceholder.Visibility = Visibility.Hidden;
			}

			// If given inputs in either fields, filter for those inputs
			if (!tbFilterUser.Text.Equals(string.Empty) || !tbFilterRole.Text.Equals(string.Empty)) {
				List<string> userIdList = _context.UserRoles
					.Where(ur =>
						// Role Filter
						(tbFilterRole.Text.Length == 0 || (ur.RoleId.Contains(tbFilterRole.Text))))
					.Select(ur => ur.UserId)
					.ToList();

				List<AgendaUser> userList = _context.Users
					.Where(u =>
						// Only unlocked and non-deleted users
						((u.LockoutEnd == null || u.LockoutEnd < DateTime.Now)
							&& u.Deleted >= DateTime.Now)

						// Role filter
						&& (userIdList.Count == 0 || userIdList.Contains(u.Id))

						// User filter
						&& (tbFilterUser.Text.Length == 0
							|| u.FirstName.Contains(tbFilterUser.Text)
							|| u.LastName.Contains(tbFilterUser.Text)))
					.OrderBy(u => u.LastName + " " + u.FirstName)
					.Select(u => u)
					.ToList();

				dgUsers.ItemsSource = userList;
				dgUsers.SelectedItem = CollectionView.NewItemPlaceholder;
				UpdateUIVisuals();
				UnselectAllWithoutTriggeringEvent();
			}
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
		private void UpdateUIVisuals() {
			foreach (ListBoxItem item in lbRoles.Items) {
				string? role = item.Content.ToString();

				if (role != null) {
					if (User != null && GetUserRoles(User).Contains(role)) {
						item.Style = (Style) FindResource("CheckedListBoxItem");
					} else {
						item.Style = null;
					}
				}
			}
		}

		// Refresh the appointments DataGrid with current data from the database
		public void UpdateDataGrid() {
			dgUsers.ItemsSource = _context.Users
				.Where(u =>
					// Only unlocked and non-deleted users
					(u.LockoutEnd == null || u.LockoutEnd < DateTime.Now)
						&& u.Deleted >= DateTime.Now)
				.OrderBy(u => u.LastName + " " + u.FirstName)
				.ToList();

			UpdateUIVisuals();
		}
	}
}
