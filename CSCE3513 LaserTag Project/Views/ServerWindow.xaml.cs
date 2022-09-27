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
        }
    }
}
