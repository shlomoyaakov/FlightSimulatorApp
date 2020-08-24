using System;
using System.Threading;
using System.Windows;
using System.ComponentModel;

namespace FlightSimulatorApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Boolean serverLag = false;
        private Boolean connectClicked = false;
        IFlightGearViewModel viewModel;
        public double Latitude
        {
            get;
            set;
        }
        public double Altitude
        {
            get;
            set;
        }

        public MainWindow()
        {
            InitializeComponent();
            ip_textbox.SelectedText = System.Configuration.ConfigurationManager.AppSettings["ip"];
            port_textbox.SelectedText = System.Configuration.ConfigurationManager.AppSettings["port"];
            disconnect_button.IsEnabled = false;
            this.viewModel = (Application.Current as App).SimulatorViewModel;
            UpdateViewModel();
            UpdateDataContext();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
        private void Network_config_MouseClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                connectClicked = true;
                network_config.IsEnabled = false;
                try
                {
                    this.viewModel.VM_connect();
                    Connected();
                }
                catch (Exception e2)
                {
                    e2.ToString();
                    status2_label.Foreground = System.Windows.Media.Brushes.Red;
                    status2_label.Content = "Unable to connect, Please check ip and port";
                    network_config.IsEnabled = true;
                }
            });
        }

        private void Disconnect_button_Click(object sender, RoutedEventArgs e)
        {
            // when disconnected button is clicked, change the interfave of the app:   
            this.viewModel.VM_disconnect();
        }

        private void UpdateView(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("VM_DashboardInfo"))
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (serverLag == true)
                    {
                        serverLag = false;
                        status2_label.Foreground = System.Windows.Media.Brushes.Green;
                        status2_label.Content = "connected";
                    }
                });
                dashBoard.SetDashBoard();
            }
            if (e.PropertyName.Equals("VM_PositionInfo"))
            {
                map.SetPlanePosition();
            }
            if (e.PropertyName.Equals("VM_ServerDisconnected"))
            {
                ServerDisconnected(true);
            }
            if (e.PropertyName.Equals("VM_DisconnectedSucceed"))
            {
                ServerDisconnected(false);
            }
            if (e.PropertyName.Equals("VM_ServerTimeOut"))
            {
                    this.Dispatcher.Invoke(() =>
                    {                    
                        serverLag = true;
                        status2_label.Foreground = System.Windows.Media.Brushes.Orange;
                        status2_label.Content = "Server lagging, maybe try to reconnect";
                    });
            }
        }
        private void UpdateViewModel()
        {
            // register functions to propertyChanged event
            viewModel.PropertyChanged += UpdateView;
            mainController.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                this.viewModel.VM_setRudderElevator();
            };
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            System.Environment.Exit(0);
        }
        public void Connected()
        {
            disconnect_button.IsEnabled = true;
            status2_label.Foreground = System.Windows.Media.Brushes.Green;
            status2_label.Content = "connected";
        }
        private void ServerDisconnected(Boolean serverFall)
        {
            this.Dispatcher.Invoke(() =>
            {              
                map.invalid_label.Content = "";
                network_config.IsEnabled = true;
                disconnect_button.IsEnabled = false;
                connectClicked = false;
                if (serverFall)
                {
                    new Thread(delegate ()
                    {
                        for (int i = 10; i > 0; i--)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                if (!connectClicked)
                                {
                                    status2_label.Foreground = System.Windows.Media.Brushes.Orange;
                                    status2_label.Content = "Server disconnected, Try to reconnect in: " + i + " seconds";
                                }
                                else
                                {
                                    i = 0;
                                }

                            });
                            Thread.Sleep(1000);
                        }
                        this.Dispatcher.Invoke(() =>
                        {
                            if (network_config.IsEnabled == true)
                                Network_config_MouseClick(null, null);
                        });
                    }).Start();
                    viewModel.VM_disconnect();
                }
                else {
                    status2_label.Foreground = System.Windows.Media.Brushes.Red;
                    status2_label.Content = "disconnected";
                }
                viewModel.ResetAll();
                dashBoard.SetDashBoard();
            });
        }
        private void UpdateDataContext()
        {
            this.DataContext = this.viewModel;
            this.mainController.DataContext = this.viewModel;
            this.map.DataContext = this.viewModel;
        }
    }
}