using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Maps.MapControl.WPF;

namespace FlightSimulatorApp.controls
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl
    {

        private double initialAngle;
        private Point previousPoint;
        int isFirstTime;
        public Map()
        {
            InitializeComponent();
            this.isFirstTime = 0;
            initialAngle = 45;
        }
        public void SetPlanePosition()
        {
            this.Dispatcher.Invoke(() =>
            {
                double[] coordinates = new double[2];
                coordinates[0] = MapLayer.GetPosition(plane).Latitude;
                coordinates[1] = MapLayer.GetPosition(plane).Longitude;
                // display the coordinates or invalid coordinates error
                if (coordinates[0] > -90 && coordinates[0] < 90 && coordinates[1] > -180 && coordinates[1] < 180)
                {

                    invalid_label.Foreground = Brushes.White;
                    invalid_label.Content = "latitude: " + coordinates[0].ToString("0.00") + ", longtitude: " + coordinates[1].ToString("0.00");

                }
                else
                {
                    invalid_label.Foreground = Brushes.Red;
                    invalid_label.Content = "Invalid airplane coordinates";
                }
                if (this.isFirstTime < 2)
                {
                    this.isFirstTime++;
                    this.previousPoint = new Point(coordinates[0] * Math.PI / 180, coordinates[1] * Math.PI / 180);
                }
                else
                {
                    //calculate the plane degree
                    double lat2 = coordinates[0] * Math.PI / 180;
                    double lng = coordinates[1] * Math.PI / 180;
                    double lat1 = this.previousPoint.X;
                    double dLon = (lng - this.previousPoint.Y);
                    double y = Math.Sin(dLon) * Math.Cos(lat2);
                    double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1)
                            * Math.Cos(lat2) * Math.Cos(dLon);
                    double brng = Math.Atan2(y, x);
                    brng = (brng * 180 / Math.PI);
                    brng = (brng + 360) % 360 + initialAngle;
                    plane_angle.Angle = brng;
                    this.previousPoint = new Point(coordinates[0] * Math.PI / 180, coordinates[1] * Math.PI / 180);
                }
            });
        }
    }
}