using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MovieLibrary2.Model
{
    [Serializable()]
    public class Movie : IEquatable<Movie>, IComparable
    {
        public string IMDbID { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public string UserRating { get; set; }
        public string FilePath { get; set; }
        public string ImagePath { get; set; } = "";
        [NonSerialized()] private BitmapImage _coverImage;
        public BitmapSource CoverImage { get => _coverImage; set { _coverImage = (BitmapImage)value; } }
        public long Size { get; set; }
        public int Runtime { get; set; }
        public string DisplayTitle => $"{Title} ({Year})";
        public Movie()
        {
        }

        public Movie(FileInfo file)
        {
            FilePath = file.FullName;
            Size = file.Length;
            ExtractInfo(file.Name);
            CheckImagePath();
            //LoadImage();
        }

        private void LoadImage()
        {
            _coverImage = new BitmapImage();
            _coverImage.BeginInit();
            _coverImage.UriSource = new Uri(ImagePath);
            _coverImage.EndInit();
        }

        public void ExtractInfo(string input)
        {
            string cleanData = CleanFileName(input);
            Match titleNyear = Regex.Match(cleanData, @"[0-9A-Za-z',: \-]+ \d{4}");
            string movInfo = titleNyear.Value;
            if (movInfo.Length > 5)
            {
                Year = int.Parse(movInfo.Substring(movInfo.Length - 4));
                if (!(Year > 1900 && Year <= DateTime.Today.Year))
                {
                    ExtractInfo(movInfo.Substring(0, movInfo.Length - 5));
                    return;
                }
                Title = movInfo.Substring(0, movInfo.Length - 5).Trim();
            }
            else
            {
                Title = cleanData;
                Year = -1;
            }
        }

        public string CleanFileName(string input)
        {
            input = input.Replace('.', ' ')
                .Replace("(", "")
                .Replace(")", "")
                .Replace("  ", " ")
                .Replace("[", "")
                .Replace("]", "");
            return input;
        }

        public void CheckImagePath()
        {
            ImagePath = Properties.Settings.Default.MovieCovers + Title;
            if (File.Exists(ImagePath))
            {
                return;
            }
            if (File.Exists(ImagePath + ".png"))
            {
                ImagePath += ".png";

            }
            else if (File.Exists(ImagePath + ".bmp"))
            {
                ImagePath += ".bmp";
            }
            else
            {
                ImagePath += ".jpg";
            }
            if (!File.Exists(ImagePath))
            {
                ImagePath = Properties.Settings.Default.NoCover;
            }
        }

        public string GetImageSavePath()
        {
            return Properties.Settings.Default.MovieCovers + Title;
        }

        public bool Equals(Movie other)
        {
            return other.FilePath.Equals(this.FilePath) && other.Size.Equals(this.Size);
        }

        public int CompareTo(object obj)
        {
            return Title.CompareTo(obj);
        }
    }
}
