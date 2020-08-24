using System;
using System.Windows.Controls;
namespace FlightSimulatorApp.controls
{
    /// <summary>
    /// Interaction logic for DashBoard2.xaml
    /// </summary>
    public partial class DashBoard2 : UserControl
    {
        public DashBoard2()
        {
            InitializeComponent();
        }
        // set the dashboard info of newDashBoard array to the dashboard components
        public void SetDashBoard()
        {
            int noErr;
            this.Dispatcher.Invoke(() =>
            {
                noErr = CheckError(air_speed_val);
                noErr *= CheckError(altitude_val);
                noErr *= CheckError(roll_val);
                noErr *= CheckError(pitch_val);
                noErr *= CheckError(altimeter_val);
                noErr *= CheckError(heading_deg_val);
                noErr *= CheckError(ground_speed_val);
                noErr *= CheckError(vertical_speed_val);
                if (noErr == 0)
                    errorLabel.Content = "Error occurring while sending or receiving data";
                else
                    errorLabel.Content = "";
            });
        }


        int CheckError(Label l)
        {
            string s = l.Content.ToString();
            Console.WriteLine(s);
            if (s.Contains("ERROR"))
            {
                l.Foreground = System.Windows.Media.Brushes.Red;
                return 0;
            }
            else
            {
                l.Foreground = System.Windows.Media.Brushes.Black;
                return 1;
            }
        }
    }
}