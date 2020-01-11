using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLibrary2.DataManagement
{
    public static class Settings
    {
        private static string SettingsFile => "settings.ini";
        public static string SettingsDirectory {get; private set; }
        public static string MovieDBAPIKey { get; private set; }
        public static string MoviesDirectoryPath { get; private set; }
        public static string DataFilePath { get; private set; }
        public static string MovieCovers { get; private set; }
        public static string NoCover { get; private set; }

        public static void GetExistingDirectory()
        {

            if (Properties.Settings.Default.SettingsPath != null &&
                Directory.Exists(Properties.Settings.Default.SettingsPath))
            {
                SettingsDirectory = Properties.Settings.Default.SettingsPath;
            }
            else
            {
                SettingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        public static void LoadSettings()
        {
            if (File.Exists(SettingsDirectory + @"\" + SettingsFile))
            {
                ReadFromSettingsFile();
            }
            else
            {
                CreateDefaultSettingsFile();
            }
        }

        private static void CreateDefaultSettingsFile() { }
        private static void ReadFromSettingsFile() { }
    }
}
