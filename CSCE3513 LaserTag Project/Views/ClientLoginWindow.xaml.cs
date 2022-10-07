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

        public ClientLoginWindow()
        {
            InitializeComponent();

            initilized = true;


            listener = new NetworkListener(600);
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


        }
    }
}
