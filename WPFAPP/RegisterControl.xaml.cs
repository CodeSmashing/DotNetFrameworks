using Microsoft.AspNetCore.Identity;
using Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFAPP {
	public partial class RegisterControl : UserControl {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		public event EventHandler SwapRequested = null!;
		public event EventHandler<AgendaUser> RegisterSuccess = null!;

		// Define registration requirements
		// Key: Field name, Value: Human-readable name
		public KeyValuePair<string, string>[] registerRequirements = [
			new("tbUsername", "Username"),
			new("tbFirstName", "First name"),
			new("tbLastName", "Last name"),
			new("tbEmail", "Email"),
			new("pbPassword", "Password"),
			new("pbConfirmPassword", "Password confirmation"),
		];

		public RegisterControl(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();
		}

		private void btnRegister_Click(object sender, RoutedEventArgs e) {
			// Clear previous errors
			tbError.Children.Clear();

			// Validate inputs
			foreach (var field in new Control[] { tbUsername, tbFirstName, tbLastName, tbEmail, pbPassword, pbConfirmPassword }) {
				// If field is required
				KeyValuePair<string, string> fieldRequirement = registerRequirements.FirstOrDefault(kvp => kvp.Key == field.Name);
				if (fieldRequirement.Value == null) {
					continue;
				}

				// Get value and check if empty
				string? value = (string?) field.GetType().GetProperty(field is TextBox ? "Text" : "Password")?.GetValue(field);
				bool isEmpty = string.IsNullOrEmpty(value);
				field.ClearValue(Border.BorderBrushProperty);

				// If empty, add error
				if (isEmpty) {
					field.BorderBrush = Brushes.Red;
					tbError.Children.Add(new TextBlock { Text = $"{fieldRequirement.Value} is required" });
				}

				// Special case: Check if passwords match
				if (field is PasswordBox passwordBox && !passwordBox.Equals(pbConfirmPassword)) {
					if (!pbConfirmPassword.Password.Equals(passwordBox.Password)) {
						field.BorderBrush = Brushes.Red;
						tbError.Children.Add(new TextBlock { Text = "Passwords aren't the same" });
					}
				}
			}

			// If there are errors, return early
			if (tbError.Children.Count > 0) {
				return;
			}

			// Attempt to create user
			AgendaUser newUser = new() {
				UserName = tbUsername.Text,
				FirstName = tbFirstName.Text,
				LastName = tbLastName.Text,
				Email = tbEmail.Text,
				EmailConfirmed = true,
				LockoutEnabled = false,
				TwoFactorEnabled = false
			};
			var result = _userManager.CreateAsync(newUser, pbPassword.Password).Result;
			if (!result.Succeeded) {
				tbError.Children.Add(new TextBlock { Text = string.Join("\n", result.Errors.Select(err => err.Description)) });
				return;
			}

			// Clear previous errors and show success message
			tbError.Children.Clear();
			MessageBox.Show("Account created successfully. You can now log in.");
			_context.Add(new IdentityUserRole<string>() { RoleId = "User", UserId = newUser.Id });
			_context.SaveChanges();

			// Notify MainWindow if registration was successful
			RegisterSuccess?.Invoke(this, newUser);
		}

		// Notify MainWindow to swap to whatever control it wants
		private void btnSwap_Click(object sender, RoutedEventArgs e) {
			SwapRequested?.Invoke(this, EventArgs.Empty);
		}
	}
}
