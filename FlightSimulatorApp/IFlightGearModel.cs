using System.ComponentModel;
namespace FlightSimulatorApp
{
    public interface IFlightGearModel : IModel, INotifyPropertyChanged
    {
        string[] DashboardInfo
        {
            get;
            set;
        }
        string[] PositionInfo
        {
            get;
            set;
        }
        double[] GetPosition();
        void SetThrottle(string t);
        void SetAileron(string a);
        void SetRudderElevator(string r, string e);
        bool serverConnected();
    }
}
