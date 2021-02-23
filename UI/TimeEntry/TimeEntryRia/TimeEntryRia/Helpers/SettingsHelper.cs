using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using System.Xml;

namespace TimeEntryRia.Helpers
{
    public static class SettingsHelper
    {
        public static void SaveSetting(string key, string value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
        }

        public static string LoadSetting(string key)
        {
            if (IsolatedStorageSettings.ApplicationSettings[key] != null)
            {
                return IsolatedStorageSettings.ApplicationSettings[key] as string;
            }

            return null;
        }

        public static void SaveSetting<T>(string key, T value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = Serialize(value);
        }

        public static T LoadSetting<T>(string key)
        {
            if (IsolatedStorageSettings.ApplicationSettings[key] != null)
            {
                return (T)IsolatedStorageSettings.ApplicationSettings[key];
            }

            return default(T);
        }

        private static string Serialize<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                dcs.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Position);
            }
        }

        private static object Deserialize<T>(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException("text");
            }

            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
            using (XmlDictionaryReader reader =
                XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(text), XmlDictionaryReaderQuotas.Max))
            {

                return (T)dcs.ReadObject(reader);
            }
        }
    }
}
