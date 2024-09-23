using Newtonsoft.Json;
using System.Text;

namespace NTDLS.Persistence
{
    /// <summary>
    /// Reads/writes to the given path.
    /// </summary>
    public class PathPersistence
    {
        /// <summary>
        /// Delegate of encryption/decryption to use used after serialization and before deserialization.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public delegate byte[] EncryptionProvider(byte[] bytes);

        /// <summary>
        /// Json serializes an object and saves it to the disk.
        /// </summary>
        public static void SaveToDisk<T>(string directoryPath, string applicationName, T obj, EncryptionProvider? encryptionProvider = null)
        {
            var dataPath = Path.Combine(directoryPath, applicationName);

            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            var type = typeof(T);
            string typeName = type.IsGenericType ? type.GetGenericArguments()[0].Name : type.Name;

            var jsonBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, Formatting.Indented));
            string dataFilePath = Path.Combine(dataPath, $"{typeName}.json");

            if (encryptionProvider != null)
            {
                jsonBytes = encryptionProvider(jsonBytes);
            }

            File.WriteAllBytes(dataFilePath, jsonBytes);
        }

        /// <summary>
        /// Json serializes an object and saves it to the disk.
        /// </summary>
        public static void SaveToDisk<T>(string directoryPath, string applicationName, T obj, JsonSerializerSettings settings, EncryptionProvider? encryptionProvider = null)
        {
            var dataPath = Path.Combine(directoryPath, applicationName);

            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            var type = typeof(T);
            string typeName = type.IsGenericType ? type.GetGenericArguments()[0].Name : type.Name;

            var jsonBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, settings));
            string dataFilePath = Path.Combine(dataPath, $"{typeName}.json");

            if (encryptionProvider != null)
            {
                jsonBytes = encryptionProvider(jsonBytes);
            }

            File.WriteAllBytes(dataFilePath, jsonBytes);
        }

        /// <summary>
        /// Loads the object from disk, deserializes it ans returns the object.
        /// </summary>
        public static T? LoadFromDisk<T>(string directoryPath, string applicationName, EncryptionProvider? encryptionProvider = null)
        {
            var dataPath = Path.Combine(directoryPath, applicationName);

            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            var type = typeof(T);
            string typeName = type.IsGenericType ? type.GetGenericArguments()[0].Name : type.Name;

            string dataFilePath = Path.Combine(dataPath, $"{typeName}.json");
            if (File.Exists(dataFilePath))
            {
                var jsonBytes = File.ReadAllBytes(dataFilePath);

                if (encryptionProvider != null)
                {
                    jsonBytes = encryptionProvider(jsonBytes);
                }

                var obj = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(jsonBytes));
                return obj;
            }
            return default;
        }

        /// <summary>
        /// Loads the object from disk, deserializes it ans returns the object.
        /// </summary>
        public static T LoadFromDisk<T>(string directoryPath, string applicationName, T defaultResult, EncryptionProvider? encryptionProvider = null)
        {
            var result = LoadFromDisk<T>(directoryPath, applicationName, encryptionProvider);
            if (result == null)
            {
                return defaultResult;
            }
            return result;
        }
    }
}
