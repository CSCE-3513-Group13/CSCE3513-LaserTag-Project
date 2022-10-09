﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCE3513_LaserTag_Project.SQL
{
    internal class SQLConnection
    {

      
        public static EntityFramework framework;
        public static NpgsqlConnection SQLCon;

        public SQLConnection()
        {

            Task lasertagSQLConnection = new Task(() => this.MainAsync().GetAwaiter().GetResult());
            lasertagSQLConnection.Start();
        }


        public async Task MainAsync()
        {
            try
            {
                SQLCon = EntityFramework.CreateSQLConnection();
                await SQLCon.OpenAsync();

                await createTable();

                framework = new EntityFramework(SQLCon, true);

                
                await framework.Count();

                //await framework.addPlayer("123", "Quick","Adrian","Gould", 100, true);
                //await framework.addPlayer("1234", "Ghost", "Person", "Gould", 120, true);
                framework.displayPlayers();

                await framework.Count();
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

            Console.WriteLine("Table created");
        }


        //Loops through our SQL database to see if we have the player
        public bool doesPlayerExsist(string id, out PlayerTable player)
        {
            player = null;
            foreach (var p in framework.Players)
            {
                if (p.playerID.Equals(id))
                    return true;
            }

            return false;   
        }


    }
}
