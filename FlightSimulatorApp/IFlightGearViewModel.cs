using System.ComponentModel;
using Microsoft.Maps.MapControl.WPF;

namespace FlightSimulatorApp
{
    public interface IFlightGearViewModel : IViewModel, INotifyPropertyChanged
    {
        string[] VM_dashboardInfo
        {
            get;
        }
        double VM_Throttle
        {
            set;
        }
        double VM_Aileron
        {
            set;
        }
        string VM_ip
        {
            set;
        }
        string VM_port
        {
            set;
        }
        Location VM_PositionInfo
        {
            get;
        }
        void VM_setRudderElevator();
        void ResetAll();
        string VM_Air_Speed
        {
            get;
        }
        string VM_Altitude
        {
            get;
        }
        string VM_Roll
        {
            get;
        }
        string VM_Pitch
        {
            get;
        }
        string VM_Altimeter
        {
            get;
        }
        string VM_Heading_Degree
        {
            get;
        }
        string VM_Ground_Speed
        {
            get;
        }
        string VM_Vertical_Speed
        {
            get;
        }
    }
}