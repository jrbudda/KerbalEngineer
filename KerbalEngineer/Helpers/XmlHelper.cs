namespace KerbalEngineer.Helpers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    public static class XmlHelper
    {
        /// <summary>
        ///     Loads an object from disk.
        /// </summary>
        public static T LoadObject<T>(string path)
        {
            T obj = default(T);

            if (File.Exists(path))
            {
                try
                {
                    using (StreamReader stream = new StreamReader(path, Encoding.UTF8))
                    {
                        obj = (T)new XmlSerializer(typeof(T)).Deserialize(stream);
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.Exception(ex);
                }
            }

            return obj;
        }

        /// <summary>
        ///     Loads an object from disk.
        /// </summary>
        public static bool LoadObject<T>(string path, out T obj)
        {
            obj = LoadObject<T>(path);
            return (obj != null);
        }

        /// <summary>
        ///     Saves an object to disk.
        /// </summary>
        public static void SaveObject<T>(string path, T obj)
        {
            if (obj == null || string.IsNullOrEmpty(path))
            {
                return;
            }

            try
            {
                using (StreamWriter stream = new StreamWriter(path, false, Encoding.UTF8))
                {
                    new XmlSerializer(typeof(T)).Serialize(stream, obj);
                }
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }
    }
}