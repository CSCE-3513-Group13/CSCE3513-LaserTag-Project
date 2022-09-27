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
        public LaunchScreen()
        {
            //Start this window in the center of the screen
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
           
            InitializeComponent();
        }
    }
}
