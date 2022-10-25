using CSCE3513_LaserTag_Project.Configs;
using CSCE3513_LaserTag_Project.Messages;
using CSCE3513_LaserTag_Project.Networking;
using CSCE3513_LaserTag_Project.SQL;
using NLog.Targets.Wrappers;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CSCE3513_LaserTag_Project.Utils;

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
        private ServerConfigs configs;

        private Random rand = new Random((int)DateTime.Now.Ticks);

        private static Logger log;

        //This is the main entry point for server code
        public ServerWindow()
        {
            InitializeComponent();

            //Use this to run when window starts
            Console.WriteLine("Started Server");
            serverStarted();

            //LogManager.Configuration.AddRule(LogLevel.Debug, LogLevel.Debug, "main");
            //LogManager.Configuration.AddRule(LogLevel.Debug, LogLevel.Debug, "console");
            //LogManager.Configuration.AddRule(LogLevel.Debug, LogLevel.Debug, "wpf");
            //LogManager.ReconfigExistingLoggers();

            //Set the UI datacontext to this class
            DataContext = this;
            this.Loaded += ServerWindow_Loaded;
        }

        public void serverStarted()
        {
            //Below is the main SQL connection 
            conn = new SQLConnection();
            configs = new ServerConfigs(this, conn);

            listener = new NetworkListener(serverRecieved, 7500);
            sender = new NetworkSender();

            
        }

        private void AttachConsole()
        {
            const string target = "wpf";
            var doc = LogManager.Configuration.FindTargetByName<FlowDocumentTarget>(target)?.Document;
            if (doc == null)
            {
                var wrapped = LogManager.Configuration.FindTargetByName<WrapperTargetBase>(target);
                doc = (wrapped?.WrappedTarget as FlowDocumentTarget)?.Document;
            }
            ConsoleText.FontSize = 12;
            ConsoleText.Document = doc ?? new FlowDocument(new Paragraph(new Run("No target!")));
            ConsoleText.TextChanged += ConsoleText_TextChanged;
        }

        private void ConsoleText_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void ServerWindow_Loaded(object sender, RoutedEventArgs e)
        {


            //log.Warn("Welcome");
        }

        //Event is used to show all players in the database for server



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

        public void autoJoinTeam(PlayerItem player)
        {
            //AutoBalances team. We will be able to switch teams on command

            if (configs.redPlayers.Count == 0)
            {
                configs.redPlayers.Add(player);
                return;
            }

            if(configs.redPlayers.Count > configs.bluePlayers.Count)
            {
                configs.bluePlayers.Add(player);
            }
            else
            {
                configs.redPlayers.Add(player);
            }
            
        }

        public async void loginRequest(MessageManager data)
        {
            LoginRequest r = Utils.Utilities.Deserialize<LoginRequest>(data.messageData);


            if (string.IsNullOrEmpty(r.playerID) && string.IsNullOrEmpty(r.username))
                return;

            Console.WriteLine($"Login Request: {r.playerID} - {r.loggingIn}");
            //might as well re-verify


            bool foundAccount = conn.doesPlayerExsist(r.playerID, out PlayerItem tableOut);

            if (r.loggingIn && !foundAccount)
            {
                r.response = $"Player of id:{r.playerID} does not exsist!";
                r.foundAccount = false;

            } else if (r.loggingIn && foundAccount)
            {

                r.response = $"Welcome {tableOut.codename}! F:{tableOut.first_name} L:{tableOut.last_name}";
                this.Dispatcher.Invoke(() => autoJoinTeam(tableOut));
            }
            else if(!r.loggingIn)
            {
                int randNum = rand.Next(100000, 999999);
                await conn.addPlayer(randNum.ToString(), r.username, r.firstname, r.lastname, 0, true);

                r.response = $"User created with ID {randNum}!";
            }

            MessageManager.sendMessage(r, MessageManager.messageType.LoginRequest, data.listenerPort);
        }
    }
}
