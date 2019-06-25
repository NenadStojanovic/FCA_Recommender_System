using FCA_Recommender_System.Data;
using Microsoft.EntityFrameworkCore;
using StorageService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCA_Recommender_System.Services
{
    public class DBStorageService : IStorageService
    {
        private int movieLimit = 500;
        public int MovieLimit { get => movieLimit;
            set
            {
                MoviesCache = null;
                CategoriesCache = null;
                movieLimit = value;
            } }
        public ApplicationDbContext dbContext { get; set; }

        public IEnumerable<Movie> MoviesCache;
        public IEnumerable<Category> CategoriesCache;

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
            if (CategoriesCache == null)
                CategoriesCache = dbContext.Categories.LimitCategories(dbContext, MovieLimit).ToList();
            return CategoriesCache;
        }
        public Category GetCategory(int id)
        {
            return dbContext.Categories.LimitCategories(dbContext, MovieLimit).FirstOrDefault(c => c.ID == id);
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
            if(MoviesCache == null)
                MoviesCache = dbContext.Movies.LimitMovies(MovieLimit).ToList();
            return MoviesCache;
        }
        public Movie GetMovie(int id)
        {
            return dbContext.Movies.LimitMovies(MovieLimit).FirstOrDefault(m => m.ID == id);
        }
        public IEnumerable<Movie> GetMoviesByNames(IEnumerable<string> names)
        {
            return dbContext.Movies.LimitMovies(MovieLimit).Where(m => names.Contains(m.Name)).ToList();
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

        public void LikeMovie(string userId, int movie, bool like)
        {
            if (like)
            {
                if (!dbContext.LikedMovies.Any(lm => lm.UserId == userId && lm.MovieId == movie))
                    dbContext.LikedMovies.Add(new LikedMovies { UserId = userId, MovieId = movie });
            }
            else
            {
                var likedMovie = dbContext.LikedMovies.FirstOrDefault(lm => lm.UserId == userId && lm.MovieId == movie);
                if (likedMovie != null)
                    dbContext.LikedMovies.Remove(likedMovie);
            }
            dbContext.SaveChanges();

        }
        public bool IsLiked(string userId, int movie)
        {
            return dbContext.LikedMovies.Any(lm => lm.UserId == userId && lm.MovieId == movie);
        }
        public int MovieLikes(int movie)
        {
            return dbContext.LikedMovies.Count(lm => lm.MovieId == movie);
        }
        public IEnumerable<Movie> LikedMovies(string userId)
        {
            return dbContext.LikedMovies.Where(lm => lm.UserId == userId).Select(lm => lm.Movie).Include(m => m.MovieCategories).ToList();
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
            if (configuration == null)
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

    public static class StorageExtensions
    {
        public static IQueryable<Movie> LimitMovies(this DbSet<Movie> movies, int limit)
        {
            return movies
                // movies that have categories
                .Where(m => m.MovieCategories.Any())
                // max 200 movies
                .Take(limit);
        }

        public static IQueryable<Category> LimitCategories(this DbSet<Category> categories, ApplicationDbContext context, int limit)
        {
            var movies = context.Movies.LimitMovies(limit);
            var _categories = context.MovieCategories.Where(mc => movies.Contains(mc.Movie)).Select(mc => mc.CategoryId).Distinct().ToList();
            return categories.Where(c => _categories.Any(_c => c.ID == _c));
        }
    }
}
