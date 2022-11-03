using CSCE3513_LaserTag_Project.SQL;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CSCE3513_LaserTag_Project.Messages.MessageManager;

namespace CSCE3513_LaserTag_Project.Messages
{

    //Fired/sent when new 
    [ProtoContract]
    public class newPlayerActivated
    {
        [ProtoMember(20)]
        public List<playerTeam> players = new List<playerTeam>();

        public newPlayerActivated() { }
    }

    [ProtoContract]
    public class playerTeam
    {
        [ProtoMember(10)]
        public Team team;

        [ProtoMember(20)]
        public PlayerItem player;
    }

}
