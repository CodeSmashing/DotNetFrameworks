using Models;
using System.Configuration;
using System.Data;
using System.Windows;

namespace WPFAPP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AgendaDbContext context = new AgendaDbContext();
            MainWindow mainWindow = new();
            AgendaDbContext.Seeder(context);
        }
    }

}
