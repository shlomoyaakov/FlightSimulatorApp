using System;
using System.Text;
using System.Net.Sockets;

namespace FlightSimulatorApp
{
    public class Client : IClient
    {
        private TcpClient myClient;
        private NetworkStream netStream;

        public Client()
        {
            this.myClient = new TcpClient();
            myClient.ReceiveTimeout=10000;
        }
        public bool IsConnected()
        {
            return myClient.Connected;
        }
        public void Connect(string ip, string port)
        {
            // try to connect the server
            try
            {
              
                 this.myClient.Connect(ip, int.Parse(port));
                netStream = myClient.GetStream();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void Write(string command)
        {
            // write to server
            try
            {
                byte[] messageSent = Encoding.ASCII.GetBytes(command + "\n");
                netStream.Write(messageSent, 0, messageSent.Length);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public string Read()
        {
            try
            {
                // read from server
                byte[] bytes = new byte[myClient.ReceiveBufferSize];
                netStream.Read(bytes, 0, (int)myClient.ReceiveBufferSize);
                // Returns the data received from the host to the console.
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Disconnect()
        {
            //disconnect from server
            try
            {
                netStream.Close();
                myClient.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}