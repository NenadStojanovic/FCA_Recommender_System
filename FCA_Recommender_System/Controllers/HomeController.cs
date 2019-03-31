using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FCA_Recommender_System.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using RecommenderEngine.Util;
using FCA_Recommender_System.Services;
using FCA_Recommender_System.Data;
using StorageService.Models;

namespace FCA_Recommender_System.Controllers
{
    public class HomeController : Controller
    {
        InputFileManager InputFileManager = new InputFileManager();
        private readonly IStorageService StorageService;

        public HomeController(ApplicationDbContext applicationDbContext)
        {
            StorageService = new DBStorageService(applicationDbContext);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult ControlPanel()
        {
            var model = new ControlPanelViewModel();
            return View(model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        Guid.NewGuid().ToString() + ".tsv");

            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);

            }
            var res = InputFileManager.ParseFile(path).Take(20).ToList();
            FileInfo fileInfo = new FileInfo(path);
            fileInfo.Delete();
            InputFileManager.GetMoveisData(res);

            // clear
            StorageService.RemoveAllMovieCategories();
            StorageService.RemoveAllCategories();
            StorageService.RemoveAllMovies();

            // categories
            var categories = res
                .SelectMany(m => m.Categories).Distinct()
                .Select(c => c.Replace("http://dbpedia.org/resource/Category:", ""))
                .Select(c => new Category
                {
                    Title = c
                }).ToList();
            StorageService.AddCategories(categories);

            // movies 
            var movies = res
                .GroupBy(m => m.Name).Select(g => g.First())
                .Select(m => new Movie
                {
                    Name = m.Name,
                    Abstract = m.Abstract,
                    DataLink = m.DataLink,
                    Director = m.Director
                }).ToList();
            StorageService.AddMovies(movies);
            

            // movie categories
            var movieCategories = new List<MovieCategory>();
            foreach(var movie in movies)
            {
                var _movieCategories = res.Where(m => m.Name == movie.Name).SelectMany(m => m.Categories).Select(c => c.Replace("http://dbpedia.org/resource/Category:", ""));
                var __movieCategories = categories.Where(c => _movieCategories.Any(_c => _c == c.Title)).ToList();
                foreach(var __movieCategory in __movieCategories)
                {
                    var movieCategory = new MovieCategory
                    {
                        MovieId = movie.ID,
                        CategoryId = __movieCategory.ID
                    };
                    movieCategories.Add(movieCategory);
                }
            }
            StorageService.AddMovieCategories(movieCategories);
            
            return RedirectToAction("Index");
        }
    }
}
