using FCA_Recommender_System.Data;
using StorageService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCA_Recommender_System.Services
{
    public class DBStorageService : IStorageService
    {
        public ApplicationDbContext dbContext;

        private ConfigurationAndStatistics DefaultConfiguration
        {
            get
            {
                return new ConfigurationAndStatistics
                {
                    Neo4jConnectionString = "http://localhost:11002/db/data",
                    Neo4jUsername = "neo4j",
                    Neo4jPass = "root",
                };
            }
        }

        public DBStorageService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return dbContext.Categories.ToList();
        }
        public Category GetCategory(int id)
        {
            return dbContext.Categories.Find(id);
        }
        public void AddCategories(IEnumerable<Category> categories)
        {
            dbContext.Categories.AddRange(categories);
            dbContext.SaveChanges();
        }
        public void RemoveAllCategories()
        {
            dbContext.Categories.RemoveRange(dbContext.Categories);
            dbContext.SaveChanges();
        }

        public IEnumerable<Movie> GetAllMovies()
        {
            return dbContext.Movies.ToList();
        }
        public Movie GetMovie(int id)
        {
            return dbContext.Movies.Find(id);
        }
        public void AddMovies(IEnumerable<Movie> movies)
        {
            dbContext.Movies.AddRange(movies);
            dbContext.SaveChanges();
        }
        public void RemoveAllMovies()
        {
            dbContext.Movies.RemoveRange(dbContext.Movies);
            dbContext.SaveChanges();
        }

        public IEnumerable<MovieCategory> GetAllMovieCategories()
        {
            return dbContext.MovieCategories.ToList();
        }
        public IEnumerable<Category> GetMovieCategories(int movieId)
        {
            var movieCategoryIds = dbContext.MovieCategories.Where(mc => mc.MovieId == movieId).Select(mc => mc.CategoryId).Distinct();
            return dbContext.Categories.Where(c => movieCategoryIds.Contains(c.ID)).ToList();
        }

        public void AddMovieCategories(IEnumerable<MovieCategory> movieCategories)
        {
            dbContext.MovieCategories.AddRange(movieCategories);
            dbContext.SaveChanges();
        }
        public void RemoveAllMovieCategories()
        {
            dbContext.MovieCategories.RemoveRange(dbContext.MovieCategories);
            dbContext.SaveChanges();
        }

        public ConfigurationAndStatistics GetConfiguration()
        {
            var configuration = dbContext.ConfigurationAndStatistics.FirstOrDefault();
            if(configuration == null)
            {
                configuration = DefaultConfiguration;
                dbContext.ConfigurationAndStatistics.Add(configuration);
                dbContext.SaveChanges();
            }
            return configuration;
        }
        public void UpdateConfiguration(ConfigurationAndStatistics configuration)
        {
            dbContext.ConfigurationAndStatistics.Update(configuration);
            dbContext.SaveChanges();
        }
    }
}
