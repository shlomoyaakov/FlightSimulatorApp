using System;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {        
            string ip = "127.0.0.1", port = "5402";
            SimulatorModel model = new SimulatorModel(new Client());
       
            try
            {
              model.connect(ip, port);
            }
            catch (Exception e) {
                Console.WriteLine("something went wrong failed\n");
            }
          
            /*
            Console.WriteLine(model.writeToServer("set /position/longitude-deg 10"));
            Console.WriteLine(model.writeToServer("set /position/latitude-deg 10"));
            Console.WriteLine(model.writeToServer("set /airspeed-indicator_indicated-speed-kt 10"));
            Console.WriteLine(model.writeToServer("set /gps_indicated-altitude-ft 10"));
            Console.WriteLine(model.writeToServer("set /attitude-indicator_internal-roll-deg 10"));
            Console.WriteLine(model.writeToServer("set /attitude-indicator_internal-pitch-deg 10"));
            Console.WriteLine(model.writeToServer("set /altimeter_indicated-altitude-ft 10"));
            Console.WriteLine(model.writeToServer("set /indicated-heading-deg 10"));
            Console.WriteLine(model.writeToServer("set /gps_indicated-ground-speed-kt 10"));
            Console.WriteLine(model.writeToServer("set /gps_indicated-vertical-speed 10"));
            Console.WriteLine(model.writeToServer("set /controls/engines/current-engine/throttle 10"));
            Console.WriteLine(model.writeToServer("set /controls/flight/aileron 10"));
            Console.WriteLine(model.writeToServer("set /controls/flight/rudder 10"));
            Console.WriteLine(model.writeToServer("set /controls/flight/elevator 10"));
            */
            Thread.Sleep(5000);
            model.disconnect();
            while (true) { }
        }
    }
}
