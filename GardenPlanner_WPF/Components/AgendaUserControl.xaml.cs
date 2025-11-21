using Microsoft.AspNetCore.Identity;
using Models;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace GardenPlanner_WPF {
	public partial class AgendaUserControl : UserControl {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		private bool _isSettingDataContext = false;
		private bool _isEditing = false;
		private bool _isSaving = false;

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

			// Set the input requirements
			inputRequirements[tbDisplayName] = "Gebruikersnaam";
			inputRequirements[tbFirstName] = "Voornaam";
			inputRequirements[tbLastName] = "Achternaam";
			inputRequirements[tbEmail] = "Email";

			// Update grids and visuals
			UpdateDataGrid();
			UpdateUIButtons();
		}

		public void dgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			grDetailsInputs.Visibility = Visibility.Collapsed;
			UpdateUIButtons();
		}

		private void tbFilter_TextChanged(object sender, TextChangedEventArgs e) {
			// Display or hide the placeholder text
			if (tbFilterUser.Text.Equals(string.Empty)) {
				tbFilterUserPlaceholder.Visibility = Visibility.Visible;
				UpdateDataGrid();
			} else {
				tbFilterUserPlaceholder.Visibility = Visibility.Hidden;
			}

			// If given inputs, filter for those inputs
			if (!tbFilterUser.Text.Equals(string.Empty)) {
				List<AgendaUser> userList = _context.Users
					.Where(u =>
						// Only unlocked and non-deleted users
						((u.LockoutEnd == null || u.LockoutEnd < DateTime.Now)
							&& u.Deleted == null)

						// User filter
						&& (tbFilterUser.Text.Length == 0
							|| u.FirstName.Contains(tbFilterUser.Text)
							|| u.LastName.Contains(tbFilterUser.Text)))
					.OrderBy(u => u.LastName + " " + u.FirstName)
					.Select(u => u)
					.ToList();

				dgUsers.ItemsSource = userList;
				dgUsers.SelectedItem = CollectionView.NewItemPlaceholder;
				UpdateUIButtons();
			}
		}

		// Handler to show save button when all details are changed
		private void grDetails_InfoChanged(object sender, EventArgs e) {
			if (_isSettingDataContext) {
				return;
			}
			btnSave.IsEnabled = MainWindow.ValidateInputs(out _, in inputRequirements);
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e) {
			grDetailsInputs.Visibility = Visibility.Visible;
			SetDataContextWithoutTriggeringEvent(CollectionView.NewItemPlaceholder);
			btnSave.IsEnabled = false;
			btnEdit.IsEnabled = false;
			_isEditing = false;
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e) {
			grDetailsInputs.Visibility = Visibility.Visible;
			SetDataContextWithoutTriggeringEvent(User);
			btnSave.IsEnabled = false;
			btnEdit.IsEnabled = false;
			btnAdd.IsEnabled = false;
			_isEditing = true;
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e) {
			try {
				AgendaUser user = (AgendaUser) dgUsers.SelectedItem;
				AgendaUser? contextUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);

				// Don't allow self deletion
				if (contextUser != null && contextUser != App.User) {
					contextUser.Deleted = DateTime.Now;
					_context.SaveChanges();

					// Reset the selected appointment and refresh the DataGrid
					dgUsers.SelectedItem = CollectionView.NewItemPlaceholder;
					UpdateDataGrid();
				}
			} catch (Exception errorInfo) {
				Console.WriteLine("Fout bij verwijderen afspraak; " + errorInfo.Message);
				UpdateDataGrid();
			}
		}

		private async void btnSave_Click(object sender, RoutedEventArgs e) {
			if (_isSaving) {
				return;
			}

			// Clear previous errors
			spError.Children.Clear();

			// Validate inputs and show errors
			MainWindow.ValidateInputs(out List<Control> errors, in inputRequirements);

			foreach (Control field in inputRequirements.Keys) {
				field.ClearValue(Border.BorderBrushProperty);

				// If invalid, add error
				if (errors.Contains(field)) {
					field.BorderBrush = Brushes.Red;
					spError.Children.Add(new TextBlock { Text = $"{inputRequirements[field]} is vereist" });
				}
			}

			// If there are errors, return early
			if (spError.Children.Count > 0) {
				return;
			}

			try {
				_isSaving = true;
				AgendaUser user;
				if (_isEditing) {
					// Attempt to update the user info
					user = _context.Users.First(u => u.Id == ((AgendaUser) grDetailsInputs.DataContext).Id);
					user.DisplayName = tbDisplayName.Text;
					user.FirstName = tbFirstName.Text;
					user.LastName = tbLastName.Text;
					user.Email = tbEmail.Text;

					if (pbPassword.Password.Length > 0) {
						// Special case: Check if passwords match
						if (!pbPasswordConfirm.Password.Equals(pbPassword.Password)) {
							pbPassword.BorderBrush = Brushes.Red;
							spError.Children.Add(new TextBlock { Text = "Wachtwoord bevestiging is niet gelijk aan het paswoord" });
							return;
						}

						// Reset password if provided
						string resetToken = await _userManager.GenerateUserTokenAsync(user, "PasswordlessLoginTotpProvider", "ResetPassword");
						IdentityResult resetResult = await _userManager.ResetPasswordAsync(user, resetToken, pbPassword.Password);
						if (!resetResult.Succeeded) {
							spError.Children.Add(new TextBlock { Text = string.Join("\n", resetResult.Errors.Select(err => err.Description)) });
							return;
						}
					}

					// Save to database
					_context.Users.Update(user);
					_context.SaveChanges();

					// Show success message
					MessageBox.Show("Gebruiker succesvol aangepast.");
					_isEditing = false;
				} else {
					// Attempt to create a new user instance
					user = new() {
						DisplayName = tbDisplayName.Text,
						FirstName = tbFirstName.Text,
						LastName = tbLastName.Text,
						Email = tbEmail.Text,
						EmailConfirmed = false,
						LockoutEnabled = false,
						TwoFactorEnabled = false
					};
					user.UserName = user.Id;

					// Validate passwords
					if (pbPassword.Password.Length == 0) {
						// If invalid, add relevant errors
						pbPassword.BorderBrush = Brushes.Red;
						spError.Children.Add(new TextBlock { Text = $"{inputRequirements[pbPassword]} is vereist" });

						if (!pbPasswordConfirm.Password.Equals(pbPassword.Password)) {
							pbPassword.BorderBrush = Brushes.Red;
							spError.Children.Add(new TextBlock { Text = "Wachtwoord bevestiging is niet gelijk aan het paswoord" });
						}
						return;
					}

					// Add to database
					var result = await _userManager.CreateAsync(user, pbPassword.Password);
					if (!result.Succeeded) {
						spError.Children.Add(new TextBlock { Text = string.Join("\n", result.Errors.Select(err => err.Description)) });
						MessageBox.Show("Gebruiker niet kunnen aanmaken.");
						return;
					}

					// Save to database
					await _userManager.AddToRoleAsync(_context.Users.First(ur => ur.Id == user.Id), "User");
					_context.SaveChanges();

					// Show success message
					MessageBox.Show("Gebruiker succesvol aangemaakt.");
				}
				grDetailsInputs.Visibility = Visibility.Collapsed;

				// Select the user and refresh the DataGrid
				UpdateDataGrid();
				dgUsers.SelectedItem = user;
				_isSaving = false;
			} catch (Exception ex) {
				MessageBox.Show(
					 $"Error: {ex.Message}\n" +
					"U zult andere waarden moeten proberen of contact opnemen met de support.",
					 "Fout details-scherm gebruiker aanmaak",
					 MessageBoxButton.OK,
					 MessageBoxImage.Error
				);
				_isSaving = false;
			}
		}

		// Refresh the users DataGrid with current data from the database
		public void UpdateDataGrid() {
			dgUsers.ItemsSource = _context.Users
				.Where(u =>
					// Only unlocked and non-deleted users
					(u.LockoutEnd == null || u.LockoutEnd < DateTime.Now)
						&& u.Deleted == null)
				.OrderBy(u => u.LastName + " " + u.FirstName)
				.ToList();

			UpdateUIButtons();
		}

		// Helper to update button visibility based on selected appointment and user state
		public void UpdateUIButtons() {
			if (App.User == null) {
				return;
			}

			// btnAdd visible when a real user is logged in
			btnAdd.IsEnabled = (App.User != AgendaUser.Dummy);

			// If no selection, nothing more to do
			if (User == CollectionView.NewItemPlaceholder || dgUsers.SelectedIndex < 0) {
				// Default states
				btnEdit.IsEnabled = false;
				btnDelete.IsEnabled = false;
				btnSave.IsEnabled = false;
				SetDataContextWithoutTriggeringEvent(CollectionView.NewItemPlaceholder);
				return;
			}

			// Only allow edit/delete to the logged-in user if they have the correct permissions
			List<string> roleList = _context.UserRoles
				.Where(ur => ur.UserId == App.User.Id)
				.Select(ur => ur.RoleId)
				.ToList();
			if (roleList.Count > 0 && (roleList.Contains("Admin") || roleList.Contains("UserAdmin"))) {
				btnEdit.IsEnabled = true;

				// Don't allow the currently logged in user from deleting themselves
				if (User != App.User) {
					btnDelete.IsEnabled = true;
				} else {
					btnDelete.IsEnabled = false;
				}
			}
			btnSave.IsEnabled = false;

			// Update the grDetails grid to use the currently selected appointment data
			SetDataContextWithoutTriggeringEvent(User);
		}

		private void SetDataContextWithoutTriggeringEvent(object item) {
			_isSettingDataContext = true;
			grDetailsInputs.DataContext = item;
			_isSettingDataContext = false;
		}
	}
}
