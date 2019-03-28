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
using StorageService.Interfaces;
using StorageService;

namespace FCA_Recommender_System.Controllers
{
    public class HomeController : Controller
    {
        InputFileManager InputFileManager = new InputFileManager();
        private readonly IStorageService StorageService;

        public HomeController()
        {
            StorageService = new DBStorageService();
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
            var res = InputFileManager.ParseFile(path);
            FileInfo fileInfo = new FileInfo(path);
            fileInfo.Delete();
            InputFileManager.GetMoviesData(res);

            // saving movies into database

            // saving categories into database

            return RedirectToAction("Index");
        }
    }
}
