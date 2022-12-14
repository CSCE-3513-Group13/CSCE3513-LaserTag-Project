using CSCE3513_LaserTag_Project.Views;
using NLog;
using NLog.Fluent;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCE3513_LaserTag_Project.SQL
{

    /// <summary>
    /// This manages all SQL related functions
    /// </summary>
    public class SQLManager
    {
        //Anything SQL related functions need to happen here
      
        private EntityFramework framework;
        public static NpgsqlConnection SQLCon;

        private static Logger log = LogManager.GetCurrentClassLogger();

        public event EventHandler<PlayerItem> newPlayer;


        public SQLManager()
        {
            Task lasertagSQLConnection = new Task(() => this.MainAsync().GetAwaiter().GetResult());
            lasertagSQLConnection.Start();

            newPlayer += SQLManager_newPlayer;
        }

        private void SQLManager_newPlayer(object sender, PlayerItem e)
        {
            //log.Info($"Added Player {e.codename}!");
            ServerWindow.Configs.AddPlayer(e);
        }

        public async Task MainAsync()
        {
            try
            {
                SQLCon = EntityFramework.CreateSQLConnection();
                await SQLCon.OpenAsync();

                await createTable();

                framework = new EntityFramework(SQLCon, true);

                //await framework.addPlayer("123", "Quick","Adrian","Gould", 100, true);
                //await framework.addPlayer("1234", "Ghost", "Person", "Gould", 120, true);
                getAllPlayers();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
        }




        public async Task createTable()
        {
            string tableCreate = $@"CREATE TABLE IF NOT EXISTS playertable(
                  playerid varchar(30) NOT NULL,
                  codename varchar(30) NOT NULL,
                  first_name VARCHAR(30) NOT NULL,
                  last_name VARCHAR(30) NOT NULL,
                  score numeric NOT NULL DEFAULT 0,
                  PRIMARY KEY(playerid)
                )";

            using (NpgsqlCommand cmd = SQLCon.CreateCommand())
            {
                cmd.CommandText = tableCreate;
                cmd.CommandType = CommandType.Text;
                await cmd.ExecuteNonQueryAsync();
            }

            log.Info("SQL Table created/Verified!");
        }


        //Loops through our SQL database to see if we have the player
        public bool doesPlayerExsist(string id, out PlayerItem player)
        {
            player = null;
            foreach (var p in framework.Players)
            {
                if (p.playerID.Equals(id))
                {
                    player = p;
                    return true;
                }
            }

            return false;   
        }


        public void getAllPlayers()
        {
            foreach (var player in framework.Players)
            {
                newPlayer.Invoke(this, player);
                //Console.WriteLine($"CodeName: {player.codename} First:{player.first_name} Last:{player.last_name} ID:{player.playerID} Score:{player.score}");
            }
        }

        public async Task addPlayer(string id, string codename, string firstname, string lastname, int score, bool save = false)
        {
            PlayerItem t = new PlayerItem();
            t.playerID = id;
            t.codename = codename;
            t.first_name = firstname;
            t.last_name = lastname;
            t.score = score;

            framework.Players.Add(t);




            if (save)
                await framework.SaveChangesAsync();

            //Invoke this event
            newPlayer.Invoke(this, t);
        }


    }
}
