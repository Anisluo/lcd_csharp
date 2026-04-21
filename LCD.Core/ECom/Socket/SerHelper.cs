using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DMSkin.Socket
{
    /// <summary>
    /// 序列化Helper
    /// </summary>
    public class SerHelper
    {
        public SerHelper()
        {
        }

        public static byte[] Serialize(object obj)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            byte[] result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                result = memoryStream.ToArray();
            }
            return result;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] buffer)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            T result;
            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                result = (T)((object)binaryFormatter.Deserialize(memoryStream));
            }
            return result;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static object Deserialize(byte[] datas, int index)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream(datas, index, datas.Length - index);
            object result = binaryFormatter.Deserialize(memoryStream);
            memoryStream.Dispose();
            return result;
        }
    }
}
