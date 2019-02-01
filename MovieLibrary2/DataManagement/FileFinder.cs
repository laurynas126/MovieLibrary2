using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLibrary2.DataManagement
{
    class FileFinder
    {
        /// <summary>
        /// Returns a list of all files in specified directory that matches search pattern
        /// <para>Separate different search patterns by '|'</para>
        /// <example>For example: "*.jpg|*.png|*.bmp" will return all jpg, png and bmp files in directory</example>
        /// </summary>
        /// <remarks>
        /// <para>Note: search pattern does not support regular expressions.</para>
        /// </remarks>
        /// <param name="initDirectory"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static List<FileInfo> FindFiles(string initDirectory, string searchPattern)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(initDirectory);
            List<FileInfo> foundFiles = new List<FileInfo>();
            List<FileInfo> badFiles = new List<FileInfo>();
            string[] patterns = searchPattern.Split('|');
            foreach (var pattern in patterns)
            {
                foundFiles.AddRange(dirinfo.GetFiles(pattern, SearchOption.AllDirectories).ToList());
            }
            foreach (FileInfo file in foundFiles)
            {
                string lowerName = file.Name.ToLower();
                if (lowerName.Contains("sample") || lowerName.Contains("rarbg.com") || lowerName == "rarbg.mp4")
                {
                    badFiles.Add(file);
                }
            }
            foreach (FileInfo file in badFiles)
            {
                foundFiles.Remove(file);
            }
            return foundFiles;
        }

        public static string GetExistingImage(string imagePath)
        {
            string[] imageFormats = { ".jpg", ".png" };
            foreach (var format in imageFormats)
            {
                if (File.Exists(imagePath + format))
                {
                    return imagePath + format;
                }
            }
            return null;
        }
    }
}
