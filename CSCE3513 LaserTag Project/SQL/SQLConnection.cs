using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCE3513_LaserTag_Project.SQL
{
    internal class SQLConnection
    {

      
        public static EntityFramework framework;

        public SQLConnection()
        {

            Task lasertagSQLConnection = new Task(() => this.MainAsync().GetAwaiter().GetResult());
            lasertagSQLConnection.Start();


        }


        public async Task MainAsync()
        {
            try
            {
                var SQLCon = EntityFramework.CreateSQLConnection();
                await SQLCon.OpenAsync();

                framework = new EntityFramework(SQLCon, true);
                Console.WriteLine("Command has been executed!");


                //await framework.Count();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
        }


    }
}
