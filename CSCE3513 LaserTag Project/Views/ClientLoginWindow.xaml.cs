using CSCE3513_LaserTag_Project.Messages;
using CSCE3513_LaserTag_Project.Networking;
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
    /// Interaction logic for ClientLoginWindow.xaml
    /// </summary>
    public partial class ClientLoginWindow : Window
    {
        private bool initilized = false;
        private NetworkListener listener;
        private NetworkSender Sender;

        public ClientLoginWindow()
        {
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
                BoxPrompt.Content = "UserID:";

            }
            else
            {
                //Display new user text
                BoxPrompt.Content = "Username:";
                UIButton.Content = "Create User";
            }
        }

        private void UIButton_Click(object sender, RoutedEventArgs e)
        {
            //This runs when user clicks that button
            LoginRequest r = new LoginRequest();

            if (IsLogin.IsChecked.Value)
            {
                r.playerID = BoxInput.Text;
            }
            else
            {
                r.username = BoxInput.Text;
            }

            //Send message to server to verify login request
            MessageManager.sendMessage(r, MessageManager.messageType.LoginRequest);
        }



        private void clientRecieved(MessageManager data)
        {
            Console.WriteLine($"ClientRecieved: {data.type}");
            switch (data.type)
            {
                case MessageManager.messageType.LoginRequest:
                    
                    break;

                case MessageManager.messageType.GameState:

                    break;



                default:
                    Console.WriteLine("Unkown network type!");
                    break;

            }
        }
    }
}
