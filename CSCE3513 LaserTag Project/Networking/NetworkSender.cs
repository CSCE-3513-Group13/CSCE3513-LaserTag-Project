using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CSCE3513_LaserTag_Project.Networking
{
    public class NetworkSender
    {
        private static int _listenerPort;
        private static int _braodcasePort = 7500;

        private static IPAddress add = IPAddress.Parse("127.0.0.1");
        private IPEndPoint endpoint;
        private Socket s;

        public static NetworkSender Sender { get; private set; }


        public NetworkSender(int targetPort = 7500)
        {
            _listenerPort = targetPort;
            endpoint = new IPEndPoint(add, _listenerPort);
            s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            Sender = this;
        }



        public void sendMessage(byte[] msg)
        {
            s.SendTo(msg, endpoint);
            Console.WriteLine($"Sent {msg.Count()} bytes to destination!");
        }


        public void sendAllMessage()
        {

        }
    }
}
