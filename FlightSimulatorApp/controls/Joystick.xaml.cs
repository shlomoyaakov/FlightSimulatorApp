using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.ComponentModel;
namespace FlightSimulatorApp.controls
{
    /// <summary>
    /// Interaction logic for Joystick.xaml
    /// </summary>
    public partial class Joystick : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private double x, y;
        private bool mousePressed;
        private Point coordinates;
        Storyboard story;
        public Point Coordinates
        {
            get
            {
                return coordinates;
            }
            set
            {
                this.coordinates = value;
                NotifyPropertyChanged("Coordinates");
            }
        }
        public Joystick()
        {
            InitializeComponent();
            this.mousePressed = false;
            story = (Storyboard)Knob.FindResource("CenterKnob");
        }
    
        private void Knob_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!mousePressed)
            {
                x = e.GetPosition(this).X;
                y = e.GetPosition(this).Y;
                (Knob).CaptureMouse();
                mousePressed = true;
            }
        }
        private void Knob_MouseMove(object sender, MouseEventArgs e)
        {
            // calcualte the value of X,Y coordinate depending on the mouse move
            if (mousePressed)
            {
                double x1, y1;
                x1 = e.GetPosition(this).X - x;
                y1 = e.GetPosition(this).Y - y;
                double radius = Base.Width / 2;
                double distance = Math.Sqrt(x1 * x1 + y1 * y1);
                if (distance <= radius)
                {
                    knobPosition.X = x1;
                    knobPosition.Y = y1;
                }
                else
                {
                    if (y1 == 0 || x1 == 0)
                    {
                        return;
                    }
                    double angle = Math.Atan(x1 / y1);
                    if (angle < 0)
                    {
                        angle *= -1;
                    }
                    knobPosition.X = radius * Math.Sin(angle);
                    knobPosition.Y = radius * Math.Cos(angle);
                    if (x1 < 0)
                    {
                        knobPosition.X *= -1;
                    }
                    if (y1 < 0)
                    {
                        knobPosition.Y *= -1;
                    }
                }
                this.Coordinates = new Point(knobPosition.X / radius, -knobPosition.Y / radius);
            }

        }
        private void Knob_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // when mouse is up, reset the coordinates
            story.Begin();
            mousePressed = false;
            knobPosition.X = 0;
            knobPosition.Y = 0;
            this.Coordinates = new Point(0, 0);
            (Knob).ReleaseMouseCapture();
        }
        private void NotifyPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private void CenterKnob_Completed(object sender, EventArgs e) {
            story.Stop();
        }
    }
}