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

namespace MovieLibrary2.ViewModel
{
    public class MoviesListView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Movie> MovieList => MovieRepository.MovieList;

        public string MovieCount => MovieList.Count + " Movies";

        public Movie SelectedMovie { get; set; }
        public Visibility DetailModeVisibility { get; set; } = Visibility.Hidden;

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
                }
                else
                {
                    ModeText = "View";
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsPreviewMode"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ModeText"));
            }
        }

        public string ModeText { get; set; } = "Edit";

        public MoviesListView() { }

        public void Update()
        {
            MovieRepository.GetMoviesFromDataFile(Properties.Settings.Default.DataFilePath);
            MovieRepository.GetMoviesFromDirectory(Properties.Settings.Default.MoviesDirectoryPath);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MovieList"));
        }

        public void MovieClick(object item)
        {
            SelectedMovie = item as Movie;
            DetailModeVisibility = Visibility.Visible;
            UpdateValues();
        }

        public void LaunchMovie()
        {
            System.Diagnostics.Process.Start(SelectedMovie.FilePath);
        }

        public void ChangeMode()
        {
            IsPreviewMode = !IsPreviewMode;
        }

        public async Task DownloadInfo()
        {
            var image = SelectedMovie.ImagePath;
            SelectedMovie.ImagePath = null;
            UpdateValues();
            File.Delete(image);
            await Task.Run(() => MovieDataDownloader.DownloadMovieData(SelectedMovie));
            UpdateValues();
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

        public void UpdateValues()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedMovie"));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MovieList.ImagePath"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DetailModeVisibility"));
        }
    }
}
