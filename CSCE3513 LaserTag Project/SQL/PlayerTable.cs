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

        [Key, Column("playerID")]
        public int Id { get; set; }

        [Key, Column("playerName")]
        public string name { get; set; }

        [Key, Column("score")]
        public int score { get; set; }


    }
}
