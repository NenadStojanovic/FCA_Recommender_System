using StorageService.Interfaces;
using StorageService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageService
{
    public class DBStorageService : IStorageService
    {
        public ApplicationDbContext dbContext;
        public void AddCategories(IEnumerable<Category> categories)
        {
            throw new NotImplementedException();
        }

        public void AddMovies(IEnumerable<Movie> movies)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Movie> GetAllMovies()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Category> GetCategories()
        {
            throw new NotImplementedException();
        }
    }
}
