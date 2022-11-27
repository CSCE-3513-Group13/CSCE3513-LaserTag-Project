using CSCE3513_LaserTag_Project.Networking;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CSCE3513_LaserTag_Project.Messages
{

    [ProtoContract]
    public class MessageManager : MessageDefinition
    {
        public enum messageType
        {
            newClient,
            LoginRequest,
            GameState,
            newPlayerActivated,
            SwitchTeam,
            GameAction
        }
        
        public enum Team
        {
            Red,
            Blue
        }

        [ProtoMember(1)]
        public messageType type;

        [ProtoMember(100)]
        public int listenerPort;

        [ProtoMember(200)]
        public byte[] messageData;





        
        public MessageManager() { }

        public MessageManager(messageType type, int listenerPort, byte[] messageData)
        {
            this.type = type;
            this.listenerPort = listenerPort;
            this.messageData = messageData;
        }


        public static void sendMessage<T>(T data, messageType type, int port = 7500)
        {
            Console.WriteLine($"Sending data on {type} to port {port}");
            //Always send listener port
            MessageManager m = new MessageManager(type, NetworkListener.listenerPort, Utils.Utilities.Serialize(data));
            NetworkSender.Sender.sendMessage(Utils.Utilities.Serialize(m), port);
        }



    }

    [ProtoContract]
    public abstract class MessageDefinition
    {



    }
}
