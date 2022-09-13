using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSCE3513_LaserTag_Project.Networking
{
    public class UDPNetworking
    {
        private static int _listenerPort = 7501;
        private static int _braodcasePort = 7500;
        public event EventHandler<string> inboundMsgs;

        private static IPAddress add = IPAddress.Parse("127.0.0.1");


        public UDPNetworking()
        {

            Task.Run(() => recieveMessages());




            sendMessage("Sending to server");
            

        }

        public void recieveMessages()
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
