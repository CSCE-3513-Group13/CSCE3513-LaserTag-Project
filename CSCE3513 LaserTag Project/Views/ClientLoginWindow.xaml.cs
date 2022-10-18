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

        private void loginRequest(MessageManager data)
        {
            LoginRequest r = Utils.Utilities.Deserialize<LoginRequest>(data.messageData);
            Console.WriteLine(r.loggingIn);
            if (r.loggingIn == true && !r.foundAccount)
            {
                
                Dispatcher.Invoke(() => UserResponse.Content = r.response);
            }
            else if(r.loggingIn == true && r.foundAccount)
            {
                Dispatcher.Invoke(() => UserResponse.Content = r.response);
            }else if(r.loggingIn == false && r.foundAccount)
            {
                Dispatcher.Invoke(() => UserResponse.Content = "Username already taken!");
            }else if(r.loggingIn == false && !r.foundAccount)
            {
                Dispatcher.Invoke(() => UserResponse.Content = r.response);
            }
        }
    }
}
