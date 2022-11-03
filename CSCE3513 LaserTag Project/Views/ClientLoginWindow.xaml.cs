using CSCE3513_LaserTag_Project.Configs;
using CSCE3513_LaserTag_Project.Messages;
using CSCE3513_LaserTag_Project.Networking;
using CSCE3513_LaserTag_Project.SQL;
using CSCE3513_LaserTag_Project.Utils;
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
using static CSCE3513_LaserTag_Project.Messages.MessageManager;

namespace CSCE3513_LaserTag_Project.Views
{
    /// <summary>
    /// Interaction logic for ClientLoginWindow.xaml
    /// </summary>
    public partial class ClientLoginWindow : Window
    {
        private bool initilized = false;
        private NetworkListener listener;
        private NetworkSender Sender;
        private string clientID;

        public static ClientConfigs Configs = new ClientConfigs();


        public ClientLoginWindow()
        {
            this.DataContext = Configs;

            InitializeComponent();

            initilized = true;

            //Client needs to send data to server

            listener = new NetworkListener(clientRecieved);
            Sender = new NetworkSender();

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!initilized)
                return;

            if (IsLogin.IsChecked.Value)
            {
                //Display login text
                UIButton.Content = "Login";
                BoxPrompt1.Content = "UserID:";

                BoxPrompt2.Visibility = Visibility.Hidden;
                BoxPrompt3.Visibility = Visibility.Hidden;
                BoxInput2.Visibility = Visibility.Hidden;
                BoxInput3.Visibility = Visibility.Hidden;
            }
            else
            {
                //Display new user text
                BoxPrompt1.Content = "Codename:";
                UIButton.Content = "Create User";


                BoxPrompt2.Visibility = Visibility.Visible;
                BoxPrompt3.Visibility = Visibility.Visible;
                BoxInput2.Visibility = Visibility.Visible;
                BoxInput3.Visibility = Visibility.Visible;
            }
        }

        private void UIButton_Click(object sender, RoutedEventArgs e)
        {
            //This runs when user clicks that button
            LoginRequest r = new LoginRequest();

            //No need to send response if box is empty
            if (string.IsNullOrEmpty(BoxInput1.Text))
                return;


            if (IsLogin.IsChecked.Value)
            {
                r.playerID = BoxInput1.Text;
                r.loggingIn = true;
            }
            else
            {
                r.username = BoxInput1.Text;
                r.loggingIn = false;

                r.firstname = BoxInput2.Text;
                r.lastname = BoxInput3.Text;

            }

            //Send message to server to verify login request
            MessageManager.sendMessage(r, MessageManager.messageType.LoginRequest);
        }



        private void clientRecieved(MessageManager data)
        {
            Console.WriteLine($"ClientRecieved: {data.type}");


            Dispatcher.Invoke(() =>
            {
                switch (data.type)
                {
                    case MessageManager.messageType.LoginRequest:
                        loginRequest(data);
                        break;

                    case MessageManager.messageType.GameState:

                        break;

                    case MessageManager.messageType.newPlayerActivated:
                        newPlayerActivated(data);
                        break;



                    default:
                        Console.WriteLine("Unkown network type!");
                        break;

                }

            });
        }

        private void newPlayerActivated(MessageManager data)
        {
            newPlayerActivated r = Utils.Utilities.Deserialize<newPlayerActivated>(data.messageData);


            Configs.redPlayers.Clear();
            Configs.bluePlayers.Clear();

            foreach(var player in r.players)
            {
                if (player.team == MessageManager.Team.Red)
                {
                    Configs.redPlayers.Add(player.player);
                }
                else
                {
                    Configs.bluePlayers.Add(player.player);
                }
            }

        }

        private void loginRequest(MessageManager data)
        {
            LoginRequest r = Utils.Utilities.Deserialize<LoginRequest>(data.messageData);
            Console.WriteLine($"{r.loggingIn} {r.foundAccount}");
            if (r.loggingIn == true && !r.foundAccount)
            {

                UserResponse.Content = r.response;


            }
            else if (r.loggingIn == true && r.foundAccount)
            {
                //Logged in

                UserResponse.Content = r.response;
                ClientLoginBox.Visibility = Visibility.Hidden;
                PlayerInfoBox.Visibility = Visibility.Visible;
                GameControl.IsEnabled = true;

                Configs.playerID = r.playerID;
                Configs.codeName = r.username;
                Configs.firstName = r.firstname;
                Configs.lastName = r.lastname;
                Configs.totalScore = r.score;
                clientID = r.playerID;




            }
            else if (r.loggingIn == false && r.foundAccount)
            {
                UserResponse.Content = "Username already taken!";
            }
            else if (r.loggingIn == false && !r.foundAccount)
            {
                //Created new account

                UserResponse.Content = r.response;
                ClientLoginBox.Visibility = Visibility.Hidden;
                PlayerInfoBox.Visibility = Visibility.Visible;
                GameControl.IsEnabled = true;

                Configs.playerID = r.playerID;
                Configs.codeName = r.username;
                Configs.firstName = r.firstname;
                Configs.lastName = r.lastname;
                Configs.totalScore = r.score;
                clientID = r.playerID;

            }
        }

        private void SwitchTeamButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerItem foundplayer;
            Team newTeam;

            foundplayer = Configs.redPlayers.FirstOrDefault(x => x.playerID == clientID);
            newTeam = Team.Blue;
            if (foundplayer == null) {
                foundplayer = Configs.bluePlayers.FirstOrDefault(x => x.playerID == clientID);
                newTeam = Team.Red;
            }

            newPlayerActivated s = new newPlayerActivated();
            playerTeam t = new playerTeam();
            t.team = newTeam;
            t.player = foundplayer;

            s.players.Add(t);

            MessageManager.sendMessage(s, MessageManager.messageType.newPlayerActivated);
        }
    }
}
