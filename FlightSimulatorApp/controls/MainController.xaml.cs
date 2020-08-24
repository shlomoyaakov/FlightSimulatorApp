using System;
using System.Windows.Controls;
using System.ComponentModel;

namespace FlightSimulatorApp.controls
{
    /// <summary>
    /// Interaction logic for MainController.xaml
    /// </summary>
    public partial class MainController : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public MainController()
        {
            InitializeComponent();
            UpdateLabels();
            joystick.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName.Equals("Coordinates"))
                {
                    UpdateLabels();
                    NotifyPropertyChanged("Rudder&Elevator");
                }
            };
        }
        private void UpdateLabels()
        {
            rudder_val.Content = String.Format("{0:F2}", joystick.Coordinates.X);
            elevator_val.Content = String.Format("{0:F2}", joystick.Coordinates.Y);
        }
        private void NotifyPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
