using StorageService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FCA_Recommender_System.Services
{
    public interface IStorageService
    {
        IEnumerable<Movie> GetAllMovies();
        Movie GetMovie(int id);
        IEnumerable<Movie> GetMoviesByNames(IEnumerable<string> names);
        void AddMovies(IEnumerable<Movie> movies);
        void RemoveAllMovies();

        void LikeMovie(string userId, int movie, bool like);
        bool IsLiked(string userId, int movie);
        int MovieLikes(int movie);
        IEnumerable<Movie> LikedMovies(string userId);

        IEnumerable<Category> GetAllCategories();
        Category GetCategory(int id);
        void AddCategories(IEnumerable<Category> categories);
        void RemoveAllCategories();

        IEnumerable<MovieCategory> GetAllMovieCategories();
        IEnumerable<Category> GetMovieCategories(int movieId);

        void AddMovieCategories(IEnumerable<MovieCategory> movieCategories);
        void RemoveAllMovieCategories();

        ConfigurationAndStatistics GetConfiguration();
        void UpdateConfiguration(ConfigurationAndStatistics configuration);
    }
}
