using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GardenPlanner_WPF {
	public partial class LoginControl : UserControl {
		private readonly AgendaDbContext _context;
		private readonly UserManager<AgendaUser> _userManager;
		public event EventHandler SwapRequested = null!;
		public event EventHandler<AgendaUser> LoginSuccess = null!;

		// Define login requirements
		// Key: Field name, Value: Human-readable name
		public Dictionary<Control, string> loginRequirements = new();

		// Constructor with dependency injection
		public LoginControl(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();

			loginRequirements[tbEmail] = "Email";
			loginRequirements[pbPassword] = "Wachtwoord";
		}

		private async void btnLogin_Click(object sender, RoutedEventArgs e) {
			// Clear previous errors
			spError.Children.Clear();

			// Validate inputs
			foreach (Control field in loginRequirements.Keys) {
				// Check if the value is empty
				bool isEmpty = true;
				switch (field) {
					case TextBox textBox:
						isEmpty = textBox.Text.Length == 0;
						break;
					case PasswordBox passwordBox:
						isEmpty = passwordBox.Password.Length == 0;
						break;
					default:
						break;
				}

				field.ClearValue(Border.BorderBrushProperty);

				// If empty, add error
				if (isEmpty) {
					field.BorderBrush = Brushes.Red;
					spError.Children.Add(new TextBlock { Text = $"{loginRequirements[field]} is vereist" });
				}
			}

			// If there are errors, return early
			if (spError.Children.Count > 0) {
				return;
			}

			// Check if user exists and password is correct
			AgendaUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == tbEmail.Text);
			if (user == null) {
				spError.Children.Add(new TextBlock { Text = "Gebruiker niet gevonden" });
				return;
			}

			bool loginSuccess = await _userManager.CheckPasswordAsync(user, pbPassword.Password);
			if (!loginSuccess) {
				spError.Children.Add(new TextBlock { Text = "Ongeldige email of wachtwoord" });
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
