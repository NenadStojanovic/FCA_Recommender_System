using StorageService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FCA_Recommender_System.Services
{
    public interface IStorageService
    {
        void AddMovies(IEnumerable<Movie> movies);
        void RemoveAllMovies();
        void AddCategories(IEnumerable<Category> categories);
        void RemoveAllCategories();
        void AddMovieCategories(IEnumerable<MovieCategory> movieCategories);
        void RemoveAllMovieCategories();

        IEnumerable<Movie> GetAllMovies();
        IEnumerable<Category> GetAllCategories();
    }
}
