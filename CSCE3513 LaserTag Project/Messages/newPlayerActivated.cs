using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCE3513_LaserTag_Project.Messages
{
    public class newPlayerActivated
    {
        [ProtoMember(10)]
        public string newPlayerActivate;

        [ProtoMember(20)]
        public string username;

        [ProtoMember(30)]
        public string score;
    }
}
