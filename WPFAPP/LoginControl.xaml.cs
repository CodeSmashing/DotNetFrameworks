using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFAPP {
	public partial class LoginControl : UserControl {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		public event EventHandler SwapRequested = null!;
		public event EventHandler<AgendaUser> LoginSuccess = null!;

		// Define login requirements
		// Key: Field name, Value: Human-readable name
		public KeyValuePair<string, string>[] loginRequirements = [
			new("tbDisplayname", "Gebruikersnaam"),
			new("pbPassword", "Wachtwoord")
		];

		// Constructor with dependency injection
		public LoginControl(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();
		}

		private async void btnLogin_Click(object sender, RoutedEventArgs e) {
			// Clear previous errors
			tbError.Children.Clear();

			// Validate inputs
			foreach (var field in new Control[] { tbDisplayname, pbPassword }) {
				// If field is required
				KeyValuePair<string, string> fieldRequirement = loginRequirements.FirstOrDefault(kvp => kvp.Key == field.Name);
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
					tbError.Children.Add(new TextBlock { Text = $"{fieldRequirement.Value} is vereist" });
				}
			}

			// If there are errors, return early
			if (tbError.Children.Count > 0) {
				return;
			}

			// Check if user exists and password is correct
			AgendaUser? user = await _context.Users.FirstOrDefaultAsync(u => u.DisplayName == tbDisplayname.Text);
			if (user == null) {
				tbError.Children.Add(new TextBlock { Text = "Gebruiker niet gevonden" });
				return;
			}

			bool loginSuccess = await _userManager.CheckPasswordAsync(user, pbPassword.Password);
			if (!loginSuccess) {
				tbError.Children.Add(new TextBlock { Text = "Ongeldige gebruikersnaam of wachtwoord" });
				return;
			}

			// Notify MainWindow if successfully logged in
			LoginSuccess?.Invoke(this, user);
		}

		// Notify MainWindow to swap to whatever control it wants
		private void btnSwap_Click(object sender, RoutedEventArgs e) {
			SwapRequested?.Invoke(this, EventArgs.Empty);
		}
	}
}
