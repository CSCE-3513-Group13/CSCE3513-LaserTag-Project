using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCE3513_LaserTag_Project.Messages
{
    [ProtoContract]
    public class GameAction
    {
        public enum colorType
        {
            red,
            blue
        }

        [ProtoMember(100)]
        public colorType cType;

        [ProtoMember(200)]
        public string action;

        [ProtoMember(300)]
        public int redScore;

        [ProtoMember(400)]
        public int blueScore;

        public GameAction() { }

        public GameAction(string action, colorType c, int redScore, int BlueScore)
        {
            this.action = action;
            this.cType = c;
            this.redScore = redScore;
            this.blueScore = BlueScore;
        }

    }
}
