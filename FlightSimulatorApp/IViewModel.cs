namespace FlightSimulatorApp
{
    public interface IViewModel
    {
        void VM_connect();
        void VM_disconnect();
        void VM_InitAndOpenReceiveThread();
    }
}