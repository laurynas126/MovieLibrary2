using MovieLibrary2.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLibrary2.DataManagement
{
    public static class MovieRepository
    {
        public static ObservableCollection<Movie> _movieList;
        public static ObservableCollection<Movie> MovieList {
            get
            {
                if (_movieList == null)
                {
                    var movies = LoadMovies().OrderBy(mov => mov.Title);
                    _movieList = new ObservableCollection<Movie>(movies);

                }
                return _movieList;
            }
            set
            {
                _movieList = value;
            }
        }

        public static Func<FileInfo, Movie> movieCreator = delegate (FileInfo info)
        {
            return new Movie(info);
        };

        public static ICollection<Movie> LoadMovies()
        {
            var list = new List<Movie>();
            var dataFileMovies = GetMoviesFromDataFile(Properties.Settings.Default.DataFilePath);
            if (dataFileMovies != null && dataFileMovies.Count > 0)
                list.AddRange(dataFileMovies);

            var directoryMovies = GetMoviesFromDirectory(Properties.Settings.Default.MoviesDirectoryPath);
            list.AddRange(directoryMovies.Where(m => !list.Contains(m)));

            return list;
        }

        public static ICollection<Movie> GetMoviesFromDirectory(string directory)
        {
            return DataLoader.LoadDataFromDir(directory, "*.mkv|*.avi|*.mp4", movieCreator);
            //var unique = from mov in loadedList
            //             where !MovieList.Contains(mov)
            //             select mov;
            //foreach (var mov in unique)
            //{
            //    MovieList.Add(mov);
            //}
        }

        public static ICollection<Movie> GetMoviesFromDataFile(string dataFile)
        {
            ICollection<Movie> movieCollection = new List<Movie>();
            if (!File.Exists(dataFile))
                return null;
            var deserializedMovieList = DataSerialization.DeserializeList<Movie>(dataFile);
            if (deserializedMovieList == null)
                return null;
            foreach (var mov in deserializedMovieList)
            {
                if (File.Exists(mov.FilePath) &&
                    !movieCollection.Contains(mov))
                    movieCollection.Add(mov);
            }
            return movieCollection;
        }

        public static void SaveMoviesToDataFile(string dataFile)
        {
            DataSerialization.SerializeList<Movie>(MovieList, dataFile);
        }
    }
}
