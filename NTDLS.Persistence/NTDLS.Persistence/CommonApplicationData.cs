﻿using Newtonsoft.Json;
using static NTDLS.Persistence.PathPersistence;

namespace NTDLS.Persistence
{
    /// <summary>
    /// Reads/writes to the directory that serves as a common repository for application-specific data that is used by all users.
    /// </summary>
    public class CommonApplicationData
    {
        /// <summary>
        /// Json serializes an object and saves it to the disk.
        /// </summary>
        public static void SaveToDisk<T>(string applicationName, T obj, EncryptionProvider? encryptionProvider = null)
            => PathPersistence.SaveToDisk<T>(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), applicationName, obj, encryptionProvider);

        /// <summary>
        /// Json serializes an object and saves it to the disk.
        /// </summary>
        public static void SaveToDisk<T>(string applicationName, T obj, JsonSerializerSettings settings, EncryptionProvider? encryptionProvider = null)
            => PathPersistence.SaveToDisk<T>(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), applicationName, obj, settings, encryptionProvider);

        /// <summary>
        /// Loads the object from disk, deserializes it ans returns the object.
        /// </summary>
        public static T? LoadFromDisk<T>(string applicationName, EncryptionProvider? encryptionProvider = null)
            => PathPersistence.LoadFromDisk<T>(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), applicationName, encryptionProvider);

        /// <summary>
        /// Loads the object from disk, deserializes it ans returns the object.
        /// </summary>
        public static T LoadFromDisk<T>(string applicationName, T defaultResult, EncryptionProvider? encryptionProvider = null)
            => PathPersistence.LoadFromDisk(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), applicationName, defaultResult, encryptionProvider);
    }
}
