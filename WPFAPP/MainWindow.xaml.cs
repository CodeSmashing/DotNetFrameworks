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
                                          where app.Deleted >= DateTime.Now && app.From > DateTime.Now
                                          orderby app.From
                                          select new
                                          {
                                              app.From,
                                              app.To,
                                              app.Title,
                                              app.Description,
                                              app.AppointmentType,
                                              Todos = context.Todos
                                                  .Where(todo => todo.AppointmentId == app.Id)
                                                  .Select(todo => new { todo.Id, todo.Title, todo.Ready })
                                                  .ToList()
                                          })
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