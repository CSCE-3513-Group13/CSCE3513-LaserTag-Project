using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCE3513_LaserTag_Project.Messages
{
    /*  This is a protocontract. This class lets us assign values and serialize them over byte[]
     *  Protobuff allows us to easily serialize and deserialize these classes
     * 
     */


    [ProtoContract]
    public class LoginRequest
    {

        [ProtoMember(100)]
        public string playerID;

        [ProtoMember(200)]
        public string username;

        [ProtoMember(300)]
        public string score;

        [ProtoMember(400)]
        public bool foundAccount;

        public LoginRequest() { }

        public LoginRequest(string playerID, string username, string score)
        {
            this.playerID = playerID;
            this.username = username;
            this.score = score;
        }
    }
}
