using System.Windows;

namespace FlightSimulatorApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public SimulatorViewModel SimulatorViewModel { get; internal set; }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SimulatorViewModel = new SimulatorViewModel(new SimulatorModel(new Client()));
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
