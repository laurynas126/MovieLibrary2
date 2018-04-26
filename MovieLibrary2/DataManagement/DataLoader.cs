using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLibrary2.DataManagement
{
    public class DataLoader
    {
        public static ICollection<TResult> LoadDataFromDir<TResult>(string directory, string pattern, Func<FileInfo, TResult> objectCreator)
        {
            List<FileInfo> fi = FileFinder.FindFiles(directory, "*.mkv|*.avi|*.mp4");
            var list = new List<TResult>();
            foreach (FileInfo file in fi)
            {
                list.Add(objectCreator(file));
            }
            return list;
        }
    }
}
