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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Views.CSCE3513_LaserTag_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dispatcher dispatch;

        public MainWindow()
        {
            InitializeComponent();

            Console.WriteLine("Hello World");

            UDPNetworking net = new UDPNetworking();
            dispatch = this.Dispatcher;
            net.inboundMsgs += Net_inboundMsgs;
        }

        private void Net_inboundMsgs(object sender, string e)
        {
            dispatch.Invoke(() => { RecievedMessages.Text += $"\n {e}"; });
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(MsgToSend.Text))
                UDPNetworking.sendMessage(MsgToSend.Text);

           
        }
    }
}
