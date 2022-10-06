﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CSCE3513_LaserTag_Project.SQL
{

    [DbConfigurationType(typeof(NpgSqlConfiguration))]
    public class EntityFramework : DbContext
    {
        private static int port = 5432;
        private static string host = "ec2-54-159-175-38.compute-1.amazonaws.com";
        private static string username = "asznpiiacgjekz";
        private static string password = "372a21af2a53d448a847e6c03664e54c0180dbb7858bd5b8719b0c29a3110ea2";
        private static string database = "da120dr1sr0bll";

        //
        public EntityFramework(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) { }



        //We can define databases here
        public DbSet<PlayerTable> Players { get; set; }

        public static NpgsqlConnection CreateSQLConnection()
        {
            var cs = $"Host={host};Username={username};Password={password};Database={database};SSL Mode=Require;Trust Server Certificate=true; EntityAdminDatabase={database}";
            Console.WriteLine(cs);

            

            return new NpgsqlConnection(cs);
        }

        public async Task Count()
        {
            try
            {
                int count = await Players.CountAsync();

                Console.WriteLine("Count:" + count);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        public async Task addPlayer(string id, string name, int score, bool save = false)
        {
            PlayerTable t = new PlayerTable();
            t.playerID = id;
            t.name = name;
            t.score = score;

            Players.Add(t);




            if (save)
                await SaveChangesAsync();
            
        }

        public void displayPlayers()
        {

            foreach(var player in Players)
            {
                Console.WriteLine($"Name: {player.name} ID:{player.playerID} Score:{player.score}");
            }

        }

    }

    public class NpgSqlConfiguration : DbConfiguration
    {
        public NpgSqlConfiguration()
        {
            var name = "Npgsql";

            SetProviderFactory(providerInvariantName: name,
                               providerFactory: NpgsqlFactory.Instance);

            SetProviderServices(providerInvariantName: name,
                                provider: NpgsqlServices.Instance);



            SetDefaultConnectionFactory(connectionFactory: new NpgsqlConnectionFactory());
        }
    }
}