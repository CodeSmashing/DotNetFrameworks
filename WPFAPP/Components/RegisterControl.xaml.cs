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
		public Dictionary<Control, string> registerRequirements = new();

		public RegisterControl(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();

			registerRequirements[tbDisplayname] = "Gebruikersnaam";
			registerRequirements[tbFirstName] = "Voornaam";
			registerRequirements[tbLastName] = "Achternaam";
			registerRequirements[tbEmail] = "Email";
			registerRequirements[pbPassword] = "Wachtwoord";
			registerRequirements[pbPasswordConfirm] = "Wachtwoord bevestiging";
		}

		private async void btnRegister_Click(object sender, RoutedEventArgs e) {
			// Clear previous errors
			spError.Children.Clear();

			// Validate inputs
			foreach (Control field in registerRequirements.Keys) {
				// Check if the value is empty
				bool isEmpty = true;
				switch (field) {
					case TextBox textBox:
						isEmpty = textBox.Text.Length == 0;
						break;
					case PasswordBox passwordBox:
						isEmpty = passwordBox.Password.Length == 0;

						// Special case: Check if passwords match
						if (!passwordBox.Equals(pbPasswordConfirm)) {
							if (!pbPasswordConfirm.Password.Equals(passwordBox.Password)) {
								field.BorderBrush = Brushes.Red;
								spError.Children.Add(new TextBlock { Text = "Wachtwoord bevestiging is niet gelijk aan het paswoord" });
							}
						}
						break;
					default:
						break;
				}

				field.ClearValue(Border.BorderBrushProperty);

				// If empty, add error
				if (isEmpty) {
					field.BorderBrush = Brushes.Red;
					spError.Children.Add(new TextBlock { Text = $"{registerRequirements[field]} is vereist" });
				}
			}

			// If there are errors, return early
			if (spError.Children.Count > 0) {
				return;
			}

			// Attempt to create user
			AgendaUser newUser = new() {
				DisplayName = tbDisplayname.Text,
				FirstName = tbFirstName.Text,
				LastName = tbLastName.Text,
				Email = tbEmail.Text,
				EmailConfirmed = true,
				LockoutEnabled = false,
				TwoFactorEnabled = false
			};
			newUser.UserName = newUser.Id;

			var result = await _userManager.CreateAsync(newUser, pbPassword.Password);
			if (!result.Succeeded) {
				spError.Children.Add(new TextBlock { Text = string.Join("\n", result.Errors.Select(err => err.Description)) });
				return;
			}

			// Clear previous errors and show success message
			spError.Children.Clear();
			MessageBox.Show("Account succesvol aangemaakt. U kunt nu inloggen.");
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
