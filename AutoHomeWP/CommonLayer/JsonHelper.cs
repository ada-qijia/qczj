
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

            try
            {
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
            catch
            {
                return null;
            }
        }

        public static T DeserializeOrDefault<T>(string jsonString)
        {
            T ret;
            try
            {
                using (MemoryStream ms = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(jsonString)))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    ret = (T)serializer.ReadObject(ms);
                }
            }
            catch
            {
                ret = default(T);
            }

            return ret;
        }
    }
}
