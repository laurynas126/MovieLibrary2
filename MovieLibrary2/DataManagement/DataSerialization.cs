using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace MovieLibrary2.DataManagement
{
    public static class DataSerialization
    {
        public static void SerializeList<T>(ICollection<T> list, string file)
        {
            IFormatter format = new BinaryFormatter();
            Stream writer = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);
            format.Serialize(writer, list);
            writer.Close();
        }

        public static ICollection<T> DeserializeList<T>(string file)
        {
            ICollection<T> list = null;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream reader = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                list = (ICollection<T>)formatter.Deserialize(reader);
                reader.Close();
            }
            catch (IOException)
            {
                MessageBox.Show("File does not exist");
            }
            return list;
        }
    }
}
