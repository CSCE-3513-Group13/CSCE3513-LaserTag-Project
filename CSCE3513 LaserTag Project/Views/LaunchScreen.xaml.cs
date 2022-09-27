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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CSCE3513_LaserTag_Project.Views
{
    /// <summary>
    /// Interaction logic for LaunchScreen.xaml
    /// </summary>
    public partial class LaunchScreen : Window
    {
        private static Window Window;


        public LaunchScreen()
        {
            //Start this window in the center of the screen
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
           
            InitializeComponent();
        }


        //This event fires when user presses client button
        private void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            //Create client window
            //Close this window

            Window = new ClientWindow();
            


            //Closes window
            this.Close();
            Window.ShowDialog();

        }

        //This event fires when user presses server button
        private void ServerButton_Click(object sender, RoutedEventArgs e)
        {
            //Create server window
            //Close this window

            Window = new ServerWindow();

            this.Close();
            Window.ShowDialog();

           
        }
    }
}
