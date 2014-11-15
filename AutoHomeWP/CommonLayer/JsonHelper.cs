
using System.Runtime.Serialization.Json;
using System.IO;

namespace CommonLayer
{
    public static class JsonHelper
    {
        public static string Serialize(object objectToSerialize)
        {
            if (objectToSerialize == null)
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(objectToSerialize.GetType());
                serializer.WriteObject(ms, objectToSerialize);
                ms.Position = 0;
                using (StreamReader reader = new StreamReader(ms))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static T Deserialize<T>(string jsonString)
        {
            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(jsonString)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }

        public static T DeserializeOrDefault<T>(string jsonString)
        {
            T ret;
            try
            {
                ret = Deserialize<T>(jsonString);
            }
            catch
            {
                ret = default(T);
            }

            return ret;
        }
    }
}
