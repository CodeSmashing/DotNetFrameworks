using Microsoft.EntityFrameworkCore;
using Models;
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

            //dgAppointments.ItemsSource = context.Appointments
            //                                        .Where(app => app.Deleted >= DateTime.Now &&
            //                                                        app.From > DateTime.Now)
            //                                        .Include(app => app.AppointmentType) 
            //                                        .ToList();

            //Anatief met query syntax(Efficient, can use only the data you need)
            dgAppointments.ItemsSource = (from app in context.Appointments
                                          where app.Deleted >= DateTime.Now && app.From > DateTime.Now
                                          orderby app.From
                                          select new { app.From, app.To, app.Title, app.Description, app.AllDay, app.AppointmentType })
                                          .ToList();
        }

        private void dgAppointments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAppointments.SelectedIndex == dgAppointments.Items.Count - 1)
            {
                btnAdd.IsEnabled = true;
                btnEdit.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
            else
            {
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnAdd.IsEnabled = false;
            }
        }
    }
}