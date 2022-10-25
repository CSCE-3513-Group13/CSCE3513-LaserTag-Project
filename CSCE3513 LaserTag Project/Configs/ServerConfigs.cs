using CSCE3513_LaserTag_Project.SQL;
using CSCE3513_LaserTag_Project.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Views.CSCE3513_LaserTag_Project;

namespace CSCE3513_LaserTag_Project.Configs
{
    public class ServerConfigs
    {
        private ServerWindow window;

        //Keep a collection of all players in SQL
        private ObservableCollection<PlayerItem> AllPlayers = new ObservableCollection<PlayerItem>();

        public ObservableCollection<PlayerItem> redPlayers = new ObservableCollection<PlayerItem>();
        public ObservableCollection<PlayerItem> bluePlayers = new ObservableCollection<PlayerItem>();

        public double timeLimit = 2.5;


        public ServerConfigs(ServerWindow window, SQLConnection conn)
        {
            this.window = window;

            window.DataContext = this;
            window.AllPlayers.ItemsSource = this.AllPlayers;
            window.redPlayers.ItemsSource = this.redPlayers;
            window.bluePlayers.ItemsSource = this.bluePlayers;

            conn.newPlayer += Conn_newPlayer;
            window.Clock.Content = TimeSpan.FromMinutes(timeLimit).ToString(@"mm\:ss");

        }

        private void Conn_newPlayer(object sender, PlayerItem e)
        {

            window.Dispatcher.Invoke(() => { AllPlayers.Add(e); });

        }
    }
}
