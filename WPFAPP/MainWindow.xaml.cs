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
		private readonly UserManager<AgendaUser> _userManager;
		private readonly LoginControl loginControl;
		private readonly RegisterControl registerControl;
		private readonly RoleControl roleControl;

		public MainWindow(AgendaDbContext context, UserManager<AgendaUser> userManager) {
			_context = context;
			_userManager = userManager;
			InitializeComponent();

			// Instantiate controls and their containers
			loginControl = new(_context, _userManager);
			registerControl = new(_context, _userManager);
			roleControl = new(_context, _userManager);
			FormContainer.Children.Clear();
			FormContainer.Children.Add(registerControl);
			UserRoleInfoContainer.Children.Add(roleControl);

			// Subscribe to swap events
			loginControl.SwapRequested += SwapControlsHandler;
			registerControl.SwapRequested += SwapControlsHandler;
            loginControl.LoginSucceeded += SuccessfulLoginHandler;

            // Hide and show elements based on required authentication
            dgAppointments.Visibility = Visibility.Hidden;
			tciUsers.Visibility = Visibility.Hidden;
			btnLogout.Visibility = Visibility.Hidden;
			tciGeneral.Visibility = Visibility.Hidden;
			tbUsernameInfo.Text = string.Empty;

            // Load appointments data
			UpdateDgAppointments();

            dgAppointments.MouseDoubleClick += dgAppointments_MouseDoubleClick;

            // Load appointment types into combobox
            cbTypes.ItemsSource = _context.AppointmentTypes
                                       .Where(apt => apt.Deleted >= DateTime.Now)
                                       .ToList();
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
			App.User = AgendaUser.Dummy;
			dgAppointments.Visibility = Visibility.Hidden;
			tciRegisterLogin.Visibility = Visibility.Visible;
			tciUsers.Visibility = Visibility.Hidden;
			btnLogout.Visibility = Visibility.Hidden;
			tciGeneral.Visibility = Visibility.Hidden;
			tbUsernameInfo.Text = string.Empty;

			// Return to appointments tab
			tcNavigation.SelectedItem = tciAppointmentRequest;
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

		private void dgAppointments_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			grDetails.Visibility = Visibility.Hidden;
			btnSave.Visibility = Visibility.Hidden;

			if (dgAppointments.SelectedIndex == dgAppointments.Items.Count - 1) {
				btnEdit.IsEnabled = false;
				btnDelete.IsEnabled = false;
			} else {
				btnEdit.IsEnabled = true;
				btnDelete.IsEnabled = true;
			}
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e) {
			btnSave.Visibility = Visibility.Hidden;
			grDetails.Visibility = Visibility.Visible;
			grDetails.DataContext = new Appointment();
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e) {
			btnSave.Visibility = Visibility.Hidden;
			grDetails.Visibility = Visibility.Visible;
			grDetails.DataContext = dgAppointments.SelectedItem;
		}
		private void btnDelete_Click(object sender, RoutedEventArgs e) {
			Appointment appointment = (Appointment) dgAppointments.SelectedItem;
			Appointment? contextAppointment = _context.Appointments.FirstOrDefault(app => app.Id == appointment.Id);

			if (contextAppointment != null) {
				contextAppointment.Deleted = DateTime.Now;
				_context.SaveChanges();

                UpdateDgAppointments();
            }
		}

		private void btnSave_Click(object sender, RoutedEventArgs e) {
			try {
				Appointment appointment = new();
				Appointment contextAppointment = (Appointment) grDetails.DataContext;

				appointment.UserId = App.User.Id;
				appointment.From = contextAppointment.From;
				appointment.To = contextAppointment.To;
				appointment.Title = contextAppointment.Title;
				appointment.Description = contextAppointment.Description;
				appointment.AppointmentType = _context.AppointmentTypes
											.FirstOrDefault(apt => apt.Id == contextAppointment.AppointmentType.Id)
											?? AppointmentType.Dummy;
				if (appointment.AppointmentType == AppointmentType.Dummy) {
					throw new Exception("Ongeldig afspraaktype geselecteerd.");
                }

                appointment.AppointmentTypeId = appointment.AppointmentType.Id;

                _context.Appointments.Add(appointment);
                _context.SaveChanges();
                btnSave.Visibility = Visibility.Hidden;

                UpdateDgAppointments();
                
			} catch (Exception erorrInfo) {
				Console.WriteLine("Fout bij opslaan afspraak; " + erorrInfo.Message);
            }
		}

		private void SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			btnSave.Visibility = Visibility.Visible;
		}

		private void TextChanged(object sender, TextChangedEventArgs e) {
			btnSave.Visibility = Visibility.Visible;
		}

		private void UpdateDgAppointments() {
            dgAppointments.ItemsSource = _context.Appointments
											.Where(app => app.Deleted >= DateTime.Now
												&& app.From > DateTime.Now
												&& app.UserId == App.User.Id)
											.OrderBy(app => app.From)
											.Select(app => app)
											//.Include(app => app.AppointmentType)  // Eager loading van AppointmentType
											.ToList();
        }

		private void SuccessfulLoginHandler(object sender, EventArgs e) {
            UpdateDgAppointments();
            tcNavigation.SelectedItem = tciAppointmentRequest;
        }
    }
}