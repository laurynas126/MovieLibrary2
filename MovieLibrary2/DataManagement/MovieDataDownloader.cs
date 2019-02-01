using MovieLibrary2.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MovieLibrary2.DataManagement
{
    public static class MovieDataDownloader
    {

        public static void DownloadMovieData(Movie movie)
        {
            string posterURL = TheMovieDBParser(movie);
            //string posterURL = OMDBApiParser(Movie);
            if (File.Exists(movie.ImagePath))
            {
                return;
            }
            movie.ImagePath = FileFinder.GetExistingImage(movie.GetImageSavePath());
            if (movie.ImagePath != null)
            {
                return;
            }
            else if (posterURL != null && posterURL != string.Empty && posterURL != "N/A")
            {
                new System.Net.WebClient().DownloadFile(posterURL, movie.GetImageSavePath() + GetImageExtension(posterURL));
                movie.ImagePath = movie.GetImageSavePath() + GetImageExtension(posterURL);
            }
        }

        public static string GetImageExtension(string url)
        {
            return "." + url.Split('.').Last();
        }

        private static string GetAddressString(Movie movie)
        {
            return @"https://api.themoviedb.org/3/search/movie?"
                        + $"api_key={MovieLibrary2.Properties.Settings.Default.MovieDBAPIKey}"
                        + $"&query={movie.Title}"
                        + ((movie.Year != -1) ? $"&primary_release_year={movie.Year}" : "");
        }
        public static string TheMovieDBParser(Movie movie)
        {
            string posterURL = null;
            using (var WebClient = new System.Net.WebClient())
            {
                try
                {
                    string addressString = GetAddressString(movie);

                    var requestResult = WebClient.DownloadString(addressString);
                    JObject json = JObject.Parse(requestResult);
                    if (json.Property("results") == null)
                        return null;
                    List<JToken> tokens = json["results"].Children().ToList();
                    if (tokens == null || tokens.Count == 0)
                        return null;
                    string movieDBID = tokens[0]["id"].ToString();
                    string movieQueryString = @"https://api.themoviedb.org/3/movie/"
                        + $"{movieDBID}"
                        + $"?api_key={MovieLibrary2.Properties.Settings.Default.MovieDBAPIKey}";
                    requestResult = WebClient.DownloadString(movieQueryString);
                    json = JObject.Parse(requestResult);
                    if (tokens != null && tokens.Count > 0)
                    {
                        movie.IMDbID = json["imdb_id"].ToString();
                        movie.Description = json["overview"].ToString();
                        movie.UserRating = json["vote_average"].ToString();
                        movie.Runtime = (int)json["runtime"];
                        posterURL = @"https://image.tmdb.org/t/p/w342" + json["poster_path"].ToString();
                    }
                }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                }
                catch (WebException ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                }
            }
            return posterURL;
        }

        public static string OMDBApiParser(Movie movie)
        {
            string posterURL = null;
            using (var WebClient = new System.Net.WebClient())
            {
                try
                {
                    string addressString = @"http://www.omdbapi.com/?s=" + movie.Title;
                    addressString += (movie.Year != -1) ? "&y=" + movie.Year : "";
                    var str = WebClient.DownloadString(addressString);
                    JObject json = JObject.Parse(str);
                    if (json.Property("Search") != null)
                    {
                        List<JToken> tokens = json["Search"].Children().ToList();
                        foreach (JToken t in tokens)
                        {
                            posterURL = (string)t["Poster"];
                            break;
                        }
                    }

                }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                }
                catch (WebException ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                }
            }
            return posterURL;
        }
    }
}
