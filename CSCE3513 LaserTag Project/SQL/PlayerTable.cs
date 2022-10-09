using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCE3513_LaserTag_Project.SQL
{

    [Table("playertable", Schema = "public")]
    public class PlayerTable
    {

        [Key, Column("playerid")]
        public string playerID { get; set; } = "";

        [Column("codename")]
        public string codename { get; set; } = "BossMan2";

        [Column("first_name")]
        public string first_name { get; set; } = "Test1";

        [Column("last_name")]
        public string last_name { get; set; } = "Test2";

        [Column("score")]
        public int score { get; set; } = 1;


    }
}
