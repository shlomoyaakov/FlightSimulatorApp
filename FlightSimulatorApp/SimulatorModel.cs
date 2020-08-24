using System;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;

namespace FlightSimulatorApp
{
    class SimulatorModel : IFlightGearModel
    {
        private volatile IClient myClient;
        private volatile Boolean connected;
        private Dictionary<int, string> dict;
        private volatile object balanceLock = new object();
        private Dictionary<string, string> controllerDict;
        private Dictionary<string, string> addressDict;
        Queue<string> sendQ;
        public event PropertyChangedEventHandler PropertyChanged;

        private string[] dashboardInfo;
        public string[] DashboardInfo
        {
            get { return dashboardInfo; }
            set
            {
                this.dashboardInfo = value;
                NotifyPropertyChanged("DashboardInfo");
            }
        }
        private double[] cordinates = new double[2];
        public string[] positionInfo;
        public string[] PositionInfo
        {
            get { return positionInfo; }
            set
            {
                // convert from one coordinate system to another in accordance to the server system
                try
                {
                    positionInfo = value;
                    cordinates[0] = ConverteCords(positionInfo[0]);
                    cordinates[1] = ConverteCords(positionInfo[1]);
                    NotifyPropertyChanged("PositionInfo");
                }
                catch (Exception e)
                {
                    // invalid coordinates - cant convert to double
                    Console.WriteLine(e.StackTrace);
                    cordinates[0] = 99999;
                    cordinates[1] = 99999;
                    NotifyPropertyChanged("PositionInfo");
                }
            }
        }

        public SimulatorModel(IClient c)
        {
            myClient = c;
            connected = false;
            dict = new Dictionary<int, string>();
            controllerDict = new Dictionary<string, string>();
            addressDict = new Dictionary<string, string>();
            InitDicts();
        }
        public void Connect(string ip, string port)
        {
            //try to connect the server with ip and port & myClient - tcp client
            try
            {
                this.myClient = new Client();
                myClient.Connect(ip, port);
                this.connected = true;
                InitAndOpenSendThread();
                InitAndOpenReceiveThread();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Disconnect(Exception serverDisconnect)
        {

            if (serverDisconnect == null)
            {
                // user disconnect from server      
                connected = false;            
            }
            else if (serverDisconnect.Message.Contains("time"))
            {
                // server time out
                if (connected)
                    NotifyPropertyChanged("ServerTimeOut");
            }
            else
            {
                // server down
                if (connected)
                {
                    this.connected = false;
                    NotifyPropertyChanged("ServerDisconnected");
                }
             
            }
        }

        public double[] GetPosition()
        {
            // get the coordinate of the plane in double format
            return cordinates;
        }
        public void InitAndOpenReceiveThread()
        {
            // this thread will run as long as the server connected and read all dashboard and position information - 4 reads per second          
            string[] newDashBoardInfo = new string[8];
            string[] newPositionInfo = new string[2];
            new Thread(delegate ()
            {
                while (connected)
                {
                    try
                    {
                        if (!IsLocked(balanceLock) && connected)
                        {
                            lock (balanceLock)
                            {
                                //get dashboard information
                                for (int i = 0; i < 8; i++)
                                {
                                    myClient.Write(this.dict[i]);
                                    newDashBoardInfo[i] = myClient.Read();
                                }

                                if (connected)
                                    DashboardInfo = newDashBoardInfo;
                                // get position information     
                                myClient.Write(this.dict[8]);
                                newPositionInfo[0] = myClient.Read();
                                myClient.Write(this.dict[9]);
                                newPositionInfo[1] = myClient.Read();
                                if (connected)
                                    PositionInfo = newPositionInfo;
                            }
                        }
                        if (connected)
                            Thread.Sleep(250);
                    }
                    catch (Exception e)
                    {
                        Disconnect(e);
                    }
                }
                lock (balanceLock)
                {
                    if (myClient.IsConnected())
                        myClient.Disconnect();
                    else
                        NotifyPropertyChanged("DisconnectedSucceed");
                }
            }).Start();
        }
        public void InitAndOpenSendThread()
        {
            //this thread run in the background and sent to the server commands
            new Thread(delegate ()
            {
                sendQ = new Queue<string>();
                string var;

                while (connected)
                {
                    while (sendQ.Count == 0 && connected)
                    { Thread.Sleep(100); }
                    if (connected && !IsLocked(balanceLock))
                    {
                        lock (balanceLock)
                        {
                            try
                            {
                                var = sendQ.Dequeue();
                                if (connected)
                                    this.myClient.Write(addressDict[var] + controllerDict[var]);
                                if (connected)
                                    myClient.Read();
                            }
                            catch (Exception e)
                            {
                                Disconnect(e);
                            }
                        }
                    }
                }
                lock (balanceLock)
                {
                    if (myClient.IsConnected())
                        myClient.Disconnect();
                    else
                        NotifyPropertyChanged("DisconnectedSucceed");
                }
            }).Start();
        }
        public void SetThrottle(string t)
        {
            // set the throttle
            if (connected)
                try
                {
                    controllerDict["throttle"] = t;
                    sendQ.Enqueue("throttle");
                }
                catch (Exception e2)
                {
                    Disconnect(e2);
                }
        }
        public void SetAileron(string a)
        {
            // set the aileron
            if (connected)
                try
                {
                    controllerDict["aileron"] = a;
                    sendQ.Enqueue("aileron");
                }
                catch (Exception e2)
                {
                    Disconnect(e2);
                }
        }
        public void SetRudderElevator(string r, string e)
        {
            // set the rudder adn elevator
            if (connected)
                try
                {
                    controllerDict["rudder"] = r;
                    controllerDict["elevator"] = e;
                    sendQ.Enqueue("rudder");
                    sendQ.Enqueue("elevator");
                }
                catch (Exception e2)
                {
                    Disconnect(e2);
                }
        }
        public double ConverteCords(string point)
        {
            /* this func should convert from one coordinate system to double coordinate system
             * 
            // format : "21*19*30.9N";
            double multiplier = (point.Contains("S") || point.Contains("W")) ? -1 : 1; //handle south and west
            point = Regex.Replace(point, "[^0-9.*]", ""); //remove the characters
            string[] pointArray = point.Split('*'); //split the string.      
            double degrees = Double.Parse(pointArray[0]);
            double minutes = Double.Parse(pointArray[1]) / 60;
            double seconds = Double.Parse(pointArray[2]) / 3600;
            return (degrees + minutes + seconds) * multiplier;*/

            return Double.Parse(point);
        }
        private void NotifyPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        private bool IsLocked(object o)
        {
            // check if the object o is locked
            if (!Monitor.TryEnter(o))
                return true;
            Monitor.Exit(o);
            return false;
        }
        private void InitDicts()
        {
            // initialize the address of the parameters
            this.dict[0] = "get /instrumentation/airspeed-indicator/indicated-speed-kt";
            this.dict[1] = "get /instrumentation/gps/indicated-altitude-ft";
            this.dict[2] = "get /instrumentation/attitude-indicator/internal-roll-deg";
            this.dict[3] = "get /instrumentation/attitude-indicator/internal-pitch-deg";
            this.dict[4] = "get /instrumentation/altimeter/indicated-altitude-ft";
            this.dict[5] = "get /instrumentation/heading-indicator/indicated-heading-deg";
            this.dict[6] = "get /instrumentation/gps/indicated-ground-speed-kt";
            this.dict[7] = "get /instrumentation/gps/indicated-vertical-speed";
            this.dict[8] = "get /position/latitude-deg";
            this.dict[9] = "get /position/longitude-deg";
            addressDict["rudder"] = "set /controls/flight/rudder ";
            addressDict["elevator"] = "set /controls/flight/elevator ";
            addressDict["aileron"] = "set /controls/flight/aileron ";
            addressDict["throttle"] = "set /controls/engines/current-engine/throttle ";
        }

        public bool serverConnected()
        {
            return myClient.IsConnected();
        }
    }
}