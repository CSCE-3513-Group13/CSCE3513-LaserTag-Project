using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCE3513_LaserTag_Project.Utils
{
    public class Utilities
    {
        //This class is used for protobuf to serialize & deserialize our messages between server and client

        public static byte[] Serialize<T>(T instance)
        {
            if (instance == null)
                return null;


            using (var m = new MemoryStream())
            {
                // m.Seek(0, SeekOrigin.Begin);
                Serializer.Serialize(m, instance);

                return m.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            if (data == null)
                return default(T);

            using (var m = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(m);
            }
        }

    }
}
