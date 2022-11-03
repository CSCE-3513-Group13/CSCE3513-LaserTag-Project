using CSCE3513_LaserTag_Project.SQL;
using CSCE3513_LaserTag_Project.Views;
using ProtoBuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml.Serialization;
using Views.CSCE3513_LaserTag_Project;
using static CSCE3513_LaserTag_Project.Configs.ServerConfigs;

namespace CSCE3513_LaserTag_Project.Configs
{
    public class ServerConfigs : SharedViewModel
    {


        [XmlIgnore] private Dispatcher dispatcher { get { return ServerWindow.serverDispatch; } }

        //Keep a collection of all players in SQL for UI
        [XmlIgnore] private ObservableCollection<PlayerItem> _AllPlayers = new ObservableCollection<PlayerItem>();
        [XmlIgnore] public ObservableCollection<PlayerItem> AllPlayers { get => _AllPlayers; set => SetValue(ref _AllPlayers, value); }


        //Collection of logged in RedPlayers
        [XmlIgnore] private ObservableCollection<PlayerItem> _redPlayers = new ObservableCollection<PlayerItem>();
        [XmlIgnore] public ObservableCollection<PlayerItem> redPlayers { get => _redPlayers; set => SetValue(ref _redPlayers, value); }

        //Collection of logged in BluePlayers
        [XmlIgnore] private ObservableCollection<PlayerItem> _bluePlayers = new ObservableCollection<PlayerItem>();
        [XmlIgnore] public ObservableCollection<PlayerItem> bluePlayers { get => _bluePlayers; set => SetValue(ref _bluePlayers, value); }


        //Updatable Timer
        [XmlIgnore] private string _timer = "";
        [XmlIgnore] public string timer { get => _timer; set => SetValue(ref _timer, value); }

        //Total Logged in players
        [XmlIgnore] private int _loggedInCount = 0;
        [XmlIgnore] public int loggedInCount { get => _loggedInCount; set => SetValue(ref _loggedInCount, value); }





        private double _timeLimit = 2.5;
        public double timeLimit { get => _timeLimit; set { SetValue(ref _timeLimit, value); updateClock(); } }



        private int _playerLives = 3;
        public int playerLives { get => _playerLives; set { SetValue(ref _playerLives, value); } }

        private int _playerDamage = 50;
        public int playerDamage { get => _playerDamage; set { SetValue(ref _playerDamage, value); } }

        private int _playerHealth = 200;
        public int playerHealth { get => _playerHealth; set { SetValue(ref _playerHealth, value); } }

        private double _teamScoreWin = 5000;
        public double teamScoreWin { get => _teamScoreWin; set { SetValue(ref _teamScoreWin, value); } }




        /*
        public ServerConfigs(ServerWindow window, SQLManager conn)
        {
            //window.AllPlayers.ItemsSource = this.AllPlayers;
            //window.redPlayers.ItemsSource = this.redPlayers;
            //window.bluePlayers.ItemsSource = this.bluePlayers;

            conn.newPlayer += Conn_newPlayer;
            window.Clock.Content = TimeSpan.FromMinutes(timeLimit).ToString(@"mm\:ss");

        }
        */

        public void updateClock()
        {
            timer = TimeSpan.FromMinutes(timeLimit).ToString("mm\\:ss");
        }

        public void updateClock(TimeSpan span)
        {
            timer = span.ToString("mm\\:ss");
        }

        public void AddPlayer(PlayerItem player)
        {
            if (Thread.CurrentThread == dispatcher.Thread)
            {
                AllPlayers.Add(player);
            }
            else
            {
                dispatcher.Invoke(() => { AllPlayers.Add(player); });
            }
        }

    }


}
