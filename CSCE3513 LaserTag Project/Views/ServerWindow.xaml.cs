using CSCE3513_LaserTag_Project.Messages;
using CSCE3513_LaserTag_Project.Networking;
using CSCE3513_LaserTag_Project.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CSCE3513_LaserTag_Project.Views
{
    /// <summary>
    /// Interaction logic for ServerWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {
        private SQLConnection conn;
        private NetworkListener listener;
        private NetworkSender sender;

        private Random rand = new Random((int)DateTime.Now.Ticks);

        //This is the main entry point for server code
        public ServerWindow()
        {
            InitializeComponent();

            //Use this to run when window starts
            Console.WriteLine("Started Server");
            serverStarted();
        }

        public void serverStarted()
        {
            //Below is the main SQL connection 
            conn = new SQLConnection();
            listener = new NetworkListener(serverRecieved, 7500);
            sender = new NetworkSender();
        }



        //This function is called when we recieve data on the server!
        private void serverRecieved(MessageManager data)
        {
            Console.WriteLine($"ServerRecieved: {data.type}");
            switch (data.type)
            {
                case MessageManager.messageType.LoginRequest:
                    loginRequest(data);
                    break;

                case MessageManager.messageType.GameState:

                    break;



                default:
                    Console.WriteLine("Unkown network type!");
                    break;

            }
        }

        public async void loginRequest(MessageManager data)
        {
            LoginRequest r = Utils.Utilities.Deserialize<LoginRequest>(data.messageData);


            if (string.IsNullOrEmpty(r.playerID) && string.IsNullOrEmpty(r.username))
                return;

            Console.WriteLine($"Login Request: {r.playerID} - {r.loggingIn}");
            //might as well re-verify


            bool foundAccount = conn.doesPlayerExsist(r.playerID, out PlayerTable tableOut);

            if (r.loggingIn && !foundAccount)
            {
                r.response = $"Player of id:{r.playerID} does not exsist!";
                r.foundAccount = false;

            } else if (r.loggingIn && foundAccount)
            {
                r.response = $"Welcome {r.username}! F:{tableOut.first_name} L:{tableOut.last_name}";
            }
            else if(!r.loggingIn)
            {
                int randNum = rand.Next(100000, 999999);
                await SQLConnection.framework.addPlayer(randNum.ToString(), r.username, r.firstname, r.lastname, 0, true);

                r.response = $"User created with ID {randNum}!";
            }

            MessageManager.sendMessage(r, MessageManager.messageType.LoginRequest, data.listenerPort);
        }
    }
}
