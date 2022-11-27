using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CSCE3513_LaserTag_Project.Messages
{
    /*  This class lets clients know if a game is active or inactive.
     * 
     * 
     */


    [ProtoContract]
    public class GameState
    {

        //datetime (start time)
        [ProtoMember(500)]
        public DateTime Start;

        //datetime (end time)
        [ProtoMember(600)]
        public DateTime End;

        //bool game state (Start/Stop)
        [ProtoMember(700)]
        public bool State;

        [ProtoMember(800)]
        public bool Reset;

    }
}
