using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WPFAPP {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private readonly AgendaDbContext _context;
		private readonly LoginControl loginControl;
		private readonly RegisterControl registerControl;

		public MainWindow(AgendaDbContext context) {
			_context = context;
			InitializeComponent();

			// Instantiate controls
			loginControl = new LoginControl(App.ServiceProvider.GetRequiredService<UserManager<AgendaUser>>());
			registerControl = new RegisterControl(App.ServiceProvider.GetRequiredService<UserManager<AgendaUser>>());
			FormContainer.Children.Clear();
			FormContainer.Children.Add(registerControl);

			// Subscribe to swap events
			loginControl.SwapRequested += SwapControlsHandler;
			registerControl.SwapRequested += SwapControlsHandler;

			// Show register by default
			FormContainer.Children.Clear();
			FormContainer.Children.Add(registerControl);

			dgAppointments.ItemsSource = (from app in context.Appointments
													orderby app.From
													select app)
													.ToList();

			dgAppointments.MouseDoubleClick += dgAppointments_MouseDoubleClick;
		}

		private void dgAppointments_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if (dgAppointments.SelectedItem is Appointment selectedAppointment) {
				// Show appointment details in a popup
				MessageBox.Show(
					 $"From: {selectedAppointment.From}\n" +
					 $"To: {selectedAppointment.To}\n" +
					 $"Title: {selectedAppointment.Title}\n" +
					 $"Description: {selectedAppointment.Description}\n" +
					 $"Type: {selectedAppointment.AppointmentType}",
					 "Appointment Details",
					 MessageBoxButton.OK,
					 MessageBoxImage.Information
				);
			}
		}

		private void btnLogout_Click(object sender, RoutedEventArgs e) {
			// Update UI for logged in user
			App.User = new();
			FormContainer.Visibility = Visibility.Visible;
			btnLogout.Visibility = Visibility.Hidden;
			tbUsernameInfo.Text = string.Empty;

			// Return to appointments tab
			tcNavigation.SelectedItem = tiAppointmentRequest;
		}

		private void SwapControlsHandler(object sender, EventArgs e) {
			// Swap between login and register controls
			if (FormContainer.Children.Contains(registerControl)) {
				// Switch to login form
				FormContainer.Children.Clear();
				FormContainer.Children.Add(loginControl);
			} else {
				// Switch to register form
				FormContainer.Children.Clear();
				FormContainer.Children.Add(registerControl);
			}
		}
	}
}