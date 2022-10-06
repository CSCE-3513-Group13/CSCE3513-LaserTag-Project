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
        private static int _listenerPort = 7501;
        private static int _braodcasePort = 7500;

        private static IPAddress add = IPAddress.Parse("127.0.0.1");

        public NetworkSender()
        {

        }

        public static void sendMessage(string msg)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);



            byte[] sendbuf = Encoding.ASCII.GetBytes(msg);
            IPEndPoint ep = new IPEndPoint(add, _listenerPort);

            s.SendTo(sendbuf, ep);

            Console.WriteLine("Message sent to the broadcast address");
        }
    }
}
