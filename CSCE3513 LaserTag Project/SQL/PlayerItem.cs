using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CSCE3513_LaserTag_Project.SQL
{
    //postgres://asznpiiacgjekz:372a21af2a53d448a847e6c03664e54c0180dbb7858bd5b8719b0c29a3110ea2@ec2-54-159-175-38.compute-1.amazonaws.com:5432/da120dr1sr0bll

    [ProtoContract]
    [Table("playertable", Schema = "public")]
    public class PlayerItem
    {
        [ProtoMember(100)]
        [Key, Column("playerid")]
        public string playerID { get; set; } = "";

        [ProtoMember(200)]
        [Column("codename")]
        public string codename { get; set; } = "BossMan2";

        [ProtoMember(300)]
        [Column("first_name")]
        public string first_name { get; set; } = "Test1";

        [ProtoMember(400)]
        [Column("last_name")]
        public string last_name { get; set; } = "Test2";

        [ProtoMember(500)]
        [Column("score")]
        public int score { get; set; } = 1;

        public PlayerItem() { }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;


            PlayerItem p = (PlayerItem)obj;
            return playerID == p.playerID;
        }


    }
}
