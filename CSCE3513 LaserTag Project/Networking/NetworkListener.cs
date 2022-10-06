using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace CSCE3513_LaserTag_Project.Networking
{
    //Network listener class. Setting up server/client connection
    public class NetworkListener
    {
        private int _listenerPort { get; }
        private static int _broadcastPort = 7500;
        public event EventHandler<string> inboundMsgs;

        private static IPAddress add = IPAddress.Parse("127.0.0.1");

        public struct UdpState
        {
            public UdpClient u;
            public IPEndPoint e;
        }


        public NetworkListener(int listenPort)
        {
            this._listenerPort = listenPort;

            startListener();
        }

        public void startListener()
        {
            IPEndPoint endpoint = new IPEndPoint(add, _listenerPort);
            UdpClient u = new UdpClient(endpoint);

            try
            {

                Console.WriteLine("Waiting for broadcast");

                UdpState s = new UdpState();
                s.e = endpoint;
                s.u = u;


                u.BeginReceive(new AsyncCallback(ReceiveCallback), s);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient u = ((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

            byte[] receiveBytes = u.EndReceive(ar, ref e);
            string receiveString = Encoding.ASCII.GetString(receiveBytes);

            Console.WriteLine($"Received: {receiveString}");
        }



    }
}
