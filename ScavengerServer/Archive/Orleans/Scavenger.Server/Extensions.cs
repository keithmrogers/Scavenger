using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Scavenger.Server
{
    public static class Extensions
    {
        public static byte[] ObjectToByteArray(this object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static object ByteArrayToObject(this byte[] arrBytes)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                object obj = (Object)binForm.Deserialize(memStream);

                return obj;
            }
        }
    }
}
