using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSCE3513_LaserTag_Project.Networking
{
    //Network listener class. Setting up server/client connection
    public class NetworkListener
    {
        private static int _listenerPort = 7501;
        private static int _broadcastPort = 7500;
        public event EventHandler<string> inboundMsgs;

        private static IPAddress add = IPAddress.Parse("127.0.0.1");

        public NetworkListener()
        {
            UdpClient listener = new UdpClient(_listenerPort);
            IPEndPoint groupEP = new IPEndPoint(add, _listenerPort);

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);

                    Console.WriteLine($"Received broadcast from {groupEP} :");
                    inboundMsgs.Invoke(this, $"Recieved '{Encoding.ASCII.GetString(bytes, 0, bytes.Length)}' from {groupEP}");


                    //Console.WriteLine($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
            }
        }
    }
}
