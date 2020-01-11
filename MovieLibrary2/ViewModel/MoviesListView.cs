using MovieLibrary2.Model;
using MovieLibrary2.DataManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Input;

namespace MovieLibrary2.ViewModel
{
    public class MoviesListView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Movie> _MovieList = MovieRepository.MovieList;
        public ObservableCollection<Movie> MovieList
        {
            get
            {
                return _MovieList;
            }
            set
            {
                _MovieList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MovieList"));
            }
        }

        public string MovieCount => MovieList.Count + " Movies";

        public Movie SelectedMovie { get; set; }
        public Visibility DetailModeVisibility { get; set; } = Visibility.Hidden;

        private string filterString = null;
        private bool _filterMode = false;
        public bool IsFilterMode
        {
            get { return _filterMode; }
            set
            {            
                if (_filterMode && !value)
                {
                    filterString = null;
                    MovieList = MovieRepository.MovieList;
                }
                _filterMode = value;
            }
        }

        private bool _previewMode = true;
        public bool IsPreviewMode
        {
            get { return _previewMode; }
            set
            {
                _previewMode = value;
                if (_previewMode)
                {
                    ModeText = "Edit";
                    TextBackground = null;
                }
                else
                {
                    ModeText = "View";
                    TextBackground = "DarkBlue";
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsPreviewMode"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ModeText"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TextBackground"));
            }
        }

        public string ModeText { get; set; } = "Edit";
        public string TextBackground { get; set; }

        public MoviesListView() { }

        public void ChangeMode() => IsPreviewMode = !IsPreviewMode;

        public async Task DownloadInfo()
        {
            await Task.Run(() => MovieDataDownloader.DownloadMovieData(SelectedMovie));
            UpdateValues();
        }

        public async Task DownloadAllMoviesInfo()
        {
            await Task.Run(() => DownloadAll());
        }

        public void DownloadAll()
        {
            foreach (var mov in MovieRepository.MovieList)
            {
                MovieDataDownloader.DownloadMovieData(mov);
                Thread.Sleep(700);
            }
        }

        public void CloseDetail()
        {
            if (DetailModeVisibility == Visibility.Visible)
            {
                SelectedMovie = null;
                DetailModeVisibility = Visibility.Hidden;
                IsPreviewMode = true;
                UpdateValues();
            }
        }

        public void LaunchMovie()
        {
            if (File.Exists(SelectedMovie.FilePath))
                System.Diagnostics.Process.Start(SelectedMovie.FilePath);
            else
                MessageBox.Show($"File {SelectedMovie.FilePath} does not exist!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void MovieClick(object item)
        {
            SelectedMovie = item as Movie;
            DetailModeVisibility = Visibility.Visible;
            UpdateValues();
        }

        public void OpenExternalLink()
        {
            string address = "";
            Movie movie = SelectedMovie;
            if (movie.IMDbID == null || movie.IMDbID == string.Empty)
                address = @"http://www.imdb.com/search/title?"
                + $"title={movie.Title}"
                + ((movie.Year != -1) ? $"&release_date={movie.Year}" : "");
            else
                address = @"http://www.imdb.com/title/"
                + $"{movie.IMDbID}";
            Uri uri = new Uri(address);
            System.Diagnostics.Process.Start(uri.AbsoluteUri);
        }

        public void Update()
        {
            MovieRepository.GetMoviesFromDataFile(Properties.Settings.Default.DataFilePath);
            MovieRepository.GetMoviesFromDirectory(Properties.Settings.Default.MoviesDirectoryPath);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MovieList"));
        }

        public void UpdateValues()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedMovie"));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MovieList.ImagePath"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DetailModeVisibility"));
        }

        public void FilterEvent(Key key)
        {
            if (DetailModeVisibility == Visibility.Visible)
            {
                return;
            }
            if (key == Key.Back)
            {
                DeleteLetter();
            }
            if (IsFilterMode && 
               (key == Key.Escape ||
               (key == Key.Back &&
                filterString == string.Empty)))
            {
                IsFilterMode = false;
                return;
            }
            else if (!IsFilterMode && IsAllowedKey(key))
            {
                IsFilterMode = true;
            }
            ApplyFilter(key);
        }

        private void ApplyFilter(Key key)
        {
            filterString += KeyToString(key);
            var filteredList = MovieRepository.MovieList.Where(mov => mov.Title.ToUpper().Contains(filterString));
            if (filteredList != null)
            {
                MovieList = new ObservableCollection<Movie>(filteredList);
            }
        }

        private void DeleteLetter()
        {
            if (filterString != null && filterString.Length > 0)
            {
                filterString = filterString.Remove(filterString.Length-1, 1);
            }
            //MessageBox.Show(filterString);
        }

        private string KeyToString(Key key)
        {

            if (key == Key.Space)
            {
                return " ";
            }
            else if (IsAllowedKey(key))
            {
                return key.ToString();
            }
            return null;
        }

        private bool IsAllowedKey(Key key)
        {
            var allowedKeys = "ABCDEFGHIJKLMNOPQRSTUWXYZ0123456789";
            return allowedKeys.Contains(key.ToString());
        }
    }
}
