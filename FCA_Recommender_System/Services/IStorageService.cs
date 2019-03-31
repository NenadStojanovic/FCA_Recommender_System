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
        void AddMovies(IEnumerable<Movie> movies);
        void RemoveAllMovies();

        IEnumerable<Category> GetAllCategories();
        Category GetCategory(int id);
        void AddCategories(IEnumerable<Category> categories);
        void RemoveAllCategories();

        IEnumerable<Category> GetMovieCategories(int movieId);

        void AddMovieCategories(IEnumerable<MovieCategory> movieCategories);
        void RemoveAllMovieCategories();
    }
}
