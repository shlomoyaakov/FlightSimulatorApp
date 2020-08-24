using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Maps.MapControl.WPF;
using System;

namespace FlightSimulatorApp
{
    public class SimulatorViewModel : IFlightGearViewModel
    {
        IFlightGearModel simulatorModel;
        public event PropertyChangedEventHandler PropertyChanged;
        string ip, port, elevator, rudder;
        private Dictionary<int, string> dict;
        string[] DashBoard = { "0", "0", "0", "0", "0", "0", "0", "0" };
        private double[] coordinates = { 32.0113888889, 34.8866666667 };
        public string VM_Elevator
        {
            set
            {
                elevator = value;
            }

        }
        public string VM_Rudder
        {
            set
            {
                rudder = value;
            }
        }
        public string[] VM_dashboardInfo
        {
            get
            {
                return this.simulatorModel.DashboardInfo;
            }
            set
            {
                this.DashBoard = value;
                for (int i = 0; i < this.DashBoard.Length; i++)
                {
                    double num;
                    try
                    {
                        num = Convert.ToDouble(this.DashBoard[i]);
                        this.DashBoard[i] = String.Format("{0:F3}", num);
                    }
                    catch (Exception)
                    {
                        this.DashBoard[i] = "ERROR";
                    }
                }
            }
        }
        public string VM_Air_Speed
        {
            get
            {
                return this.DashBoard[0];
            }
        }
        public string VM_Altitude
        {
            get
            {
                return this.DashBoard[1];
            }
        }
        public string VM_Roll
        {
            get
            {
                return this.DashBoard[2];
            }
        }
        public string VM_Pitch
        {
            get
            {
                return this.DashBoard[3];
            }
        }
        public string VM_Altimeter
        {
            get
            {
                return this.DashBoard[4];
            }
        }
        public string VM_Heading_Degree
        {
            get
            {
                return this.DashBoard[5];
            }
        }
        public string VM_Ground_Speed
        {
            get
            {
                return this.DashBoard[6];
            }
        }
        public string VM_Vertical_Speed
        {
            get
            {
                return this.DashBoard[7];
            }
        }
        public double VM_Aileron
        {
            set
            {
                this.simulatorModel.SetAileron(value.ToString());
            }
        }
        public double VM_Throttle
        {
            set
            {
                this.simulatorModel.SetThrottle(value.ToString());
            }
        }
        public string VM_ip
        {
            set
            {
                ip = value;
            }
        }
        public string VM_port
        {
            set
            {
                port = value;
            }
        }
        public Location VM_PositionInfo
        {
            get
            {
                return new Location(coordinates[0], coordinates[1]);
            }

        }
        public SimulatorViewModel(IFlightGearModel m)
        {
            this.simulatorModel = m;
            this.dict = new Dictionary<int, string>();
            this.simulatorModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName.Equals("DashboardInfo") && simulatorModel.serverConnected())
                {
                    this.VM_dashboardInfo = this.simulatorModel.DashboardInfo;
                    for (int i = 0; i < 8; i++)
                    {
                        this.NotifyPropertyChanged(this.dict[i]);
                    }
                }
                if (e.PropertyName.Equals("PositionInfo"))
                {
                    this.coordinates = this.simulatorModel.GetPosition();
                }
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
            NotifyPropertyChanged("VM_PositionInfo");
            DashBoardProperties();
        }
        public void VM_connect()
        {
            this.simulatorModel.Connect(ip, port);
        }
        public void VM_disconnect()
        {
            this.simulatorModel.Disconnect(null);            
        }
        public void ResetAll()
        {
            this.DashBoard = new string[] { "0", "0", "0", "0", "0", "0", "0", "0" };
            for (int i = 0; i < 8; i++)
            {
                this.NotifyPropertyChanged(this.dict[i]);
            }
            this.NotifyPropertyChanged("VM_DashboardInfo");
        }
        public void VM_setRudderElevator()
        {
            this.simulatorModel.SetRudderElevator(rudder, elevator);
        }
        public void VM_InitAndOpenReceiveThread()
        {
            this.simulatorModel.InitAndOpenReceiveThread();
        }
        private void NotifyPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        private void DashBoardProperties()
        {
            this.dict[0] = "VM_Air_Speed";
            this.dict[1] = "VM_Altitude";
            this.dict[2] = "VM_Roll";
            this.dict[3] = "VM_Pitch";
            this.dict[4] = "VM_Altimeter";
            this.dict[5] = "VM_Heading_Degree";
            this.dict[6] = "VM_Ground_Speed";
            this.dict[7] = "VM_Vertical_Speed";
        }
    }
}