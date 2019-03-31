using StorageService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageService.Interfaces
{
    public interface IStorageService
    {
        void AddMovies(IEnumerable<Movie> movies);
        void AddCategories(IEnumerable<Category> categories);

        IEnumerable<Movie> GetAllMovies();
        IEnumerable<Category> GetCategories();
    }
}
