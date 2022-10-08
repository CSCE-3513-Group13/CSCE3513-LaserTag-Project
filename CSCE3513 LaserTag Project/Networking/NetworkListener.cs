using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using CSCE3513_LaserTag_Project.Messages;

namespace CSCE3513_LaserTag_Project.Networking
{
    //Network listener class. Setting up server/client connection
    public class NetworkListener
    {
        public static int listenerPort { get; private set; }
        private static int _broadcastPort = 7500;

        //The async callback is something we pass into the constructor so we can delegate our callbacks from there
        private Action<MessageManager> inboundMsgCallback;

        private static IPAddress add = IPAddress.Parse("127.0.0.1");

        public struct UdpState
        {
            public UdpClient u;
            public IPEndPoint e;
        }



        public NetworkListener(Action<MessageManager> callback, int desiredPort = 0)
        {
            this.inboundMsgCallback = callback;

            //Start new background task
            Task.Run(() => startListener(desiredPort));
        }


        //Specifying 0 will give us a random free port to listen to.
        public void startListener(int desiredPort)
        {
            IPEndPoint endpoint = new IPEndPoint(add, desiredPort);
            UdpClient u = new UdpClient(endpoint);


            try
            {

                //Get the assigned random port and store it. We will need to send this to the client to keep track of its port/IP
                listenerPort = ((IPEndPoint)u.Client.LocalEndPoint).Port;
                Console.WriteLine($"Starting Network Listener {listenerPort}");

                while (true)
                {
                    byte[] receiveBytes = u.Receive(ref endpoint);

                    try
                    {
                        MessageManager m = Utils.Utilities.Deserialize<MessageManager>(receiveBytes);
                        inboundMsgCallback.Invoke(m);

                    }catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                    

                    //string receiveString = Encoding.ASCII.GetString(receiveBytes);
                    Console.WriteLine($"Received: {receiveBytes.Count()} bytes - from:{endpoint}");
                }

                
               // u.BeginReceive(new AsyncCallback(ReceiveCallback), s);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
        }





    }
}
