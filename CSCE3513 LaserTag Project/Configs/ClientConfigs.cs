using CSCE3513_LaserTag_Project.SQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CSCE3513_LaserTag_Project.Configs
{
    public class ClientConfigs : SharedViewModel
    {
        //Collection of logged in RedPlayers
        [XmlIgnore] private ObservableCollection<PlayerItem> _redPlayers = new ObservableCollection<PlayerItem>();
        [XmlIgnore] public ObservableCollection<PlayerItem> redPlayers { get => _redPlayers; set => SetValue(ref _redPlayers, value); }

        //Collection of logged in BluePlayers
        [XmlIgnore] private ObservableCollection<PlayerItem> _bluePlayers = new ObservableCollection<PlayerItem>();
        [XmlIgnore] public ObservableCollection<PlayerItem> bluePlayers { get => _bluePlayers; set => SetValue(ref _bluePlayers, value); }

        //Updatable Timer
        [XmlIgnore] private string _timer = "2:30";
        [XmlIgnore] public string timer { get => _timer; set => SetValue(ref _timer, value); }

        //Total Logged in players
        [XmlIgnore] private int _loggedInCount = 0;
        [XmlIgnore] public int loggedInCount { get => _loggedInCount; set => SetValue(ref _loggedInCount, value); }


        [XmlIgnore] private string _playerID = "";
        [XmlIgnore] public string playerID { get => _playerID; set => SetValue(ref _playerID, value); }


        [XmlIgnore] private string _codeName = "";
        [XmlIgnore] public string codeName { get => _codeName; set => SetValue(ref _codeName, value); }


        [XmlIgnore] private string _firstName = "";
        [XmlIgnore] public string firstName { get => _firstName; set => SetValue(ref _firstName, value); }


        [XmlIgnore] private string _lastName = "";
        [XmlIgnore] public string lastName { get => _lastName; set => SetValue(ref _lastName, value); }


        [XmlIgnore] private string _totalScore = "";
        [XmlIgnore] public string totalScore { get => _totalScore; set => SetValue(ref _totalScore, value); }


        public void updateClock(TimeSpan span)
        {
            timer = span.ToString("mm\\:ss");
        }

    }
}
