using System;

namespace FlightSimulatorApp
{
    public interface IModel
    {
        void Connect(string ip, string port);
        void Disconnect(Exception serverDisconnect);
        void InitAndOpenReceiveThread();
        void InitAndOpenSendThread();
    }
}
