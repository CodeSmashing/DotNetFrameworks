using Microsoft.EntityFrameworkCore;
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

namespace WPFAPP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AgendaDbContext context = new();

            dgAppointments.ItemsSource = (from app in context.Appointments
                                          orderby app.From
                                          select app)
                                          .ToList();

            dgAppointments.MouseDoubleClick += dgAppointments_MouseDoubleClick;
        }

        private void dgAppointments_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgAppointments.SelectedItem is Appointment selectedAppointment)
            {
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
    }
}