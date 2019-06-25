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
using FCAA.FormalConceptAlgorithms;
using FCAA.Data;
using Object = FCAA.Data.Object;
using Attribute = FCAA.Data.Attribute;
using FCAA.Data.Lattice;
using Neo4jFCA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using static Neo4jFCA.Neo4jDataProvider;
using System.Threading;

namespace FCA_Recommender_System.Controllers
{
    public class HomeController : Controller
    {
        InputFileManager InputFileManager = new InputFileManager();
        private readonly IStorageService StorageService;
        private readonly UserManager<ApplicationUser> userManager;

        private Neo4jDataProvider neo4JDataProvider;
        private Neo4jDataProvider Neo4JDataProvider
        {
            get
            {
                if (neo4JDataProvider != null)
                    return neo4JDataProvider;
                var configuration = StorageService.GetConfiguration();
                neo4JDataProvider = new Neo4jDataProvider(configuration.Neo4jConnectionString, configuration.Neo4jUsername, configuration.Neo4jPass);
                return neo4JDataProvider;
            }
        }

        private Thread ProcessThread { get; set; }

        public HomeController(UserManager<ApplicationUser> userManager, IStorageService storageService)
        {
            StorageService = storageService;
            this.userManager = userManager;
        }

        public string UserId => userManager.GetUserId(User);

        public IActionResult Index()
        {
            var vmodel = new HomeIndexViewModel();
            vmodel.Movies = StorageService.GetAllMovies().ToList();
            vmodel.Categories = StorageService.GetAllCategories().ToList();
            vmodel.Recomended = GetRecomendedMovies(vmodel.Movies).Take(10).ToList();

            return View(vmodel);
        }

        public IActionResult Movie(int id)
        {
            var vmodel = new MovieViewModel();
            vmodel.Movie = StorageService.GetMovie(id);
            vmodel.Categories = StorageService.GetMovieCategories(id).ToList();
            vmodel.Likes = StorageService.MovieLikes(id);
            if (User.Identity.IsAuthenticated)
            {
                vmodel.Liked = StorageService.IsLiked(UserId, id);
            }
            vmodel.RecomendedMovies = GetRecomendedMovies(id).Take(10).ToList();
            //vmodel.RecomendedMovies = StorageService.GetAllMovies().Take(10).ToList();
            return View(vmodel);
        }

        [Authorize]
        [HttpPost]
        public string LikeMovie(int id, bool like)
        {
            StorageService.LikeMovie(UserId, id, like);
            return "{}";
        }

        [Authorize]
        public IActionResult ControlPanel()
        {
            var model = new ControlPanelViewModel();
            model.ConfigurationAndStatistics = StorageService.GetConfiguration();
            return View(model);
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

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");
            var readFromDBPedia = false;
            if (readFromDBPedia)
            {
                var path = Path.Combine(
                          Directory.GetCurrentDirectory(), "wwwroot",
                          Guid.NewGuid().ToString() + ".tsv");

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);

                }
                var res = InputFileManager.ParseFile(path).ToList();
                FileInfo fileInfo = new FileInfo(path);
                fileInfo.Delete();
                InputFileManager.GetMoviesData(res);

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
                foreach (var movie in movies)
                {
                    var _movieCategories = res.Where(m => m.Name == movie.Name).SelectMany(m => m.Categories).Select(c => c.Replace("http://dbpedia.org/resource/Category:", ""));
                    var __movieCategories = categories.Where(c => _movieCategories.Any(_c => _c == c.Title)).ToList();
                    foreach (var __movieCategory in __movieCategories)
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
            }

            // Calculating lattice
            var lattice = CallculateLattice();

            var configuration = StorageService.GetConfiguration();
            configuration.LatticeCalculationTime = DateTime.Now;
            configuration.LatticeHeight = lattice.Height;
            configuration.ObjectsCount = lattice.FormalContext.ObjectsSet.Count();
            configuration.AttributesCount = lattice.FormalContext.AttributesSet.Count();
            configuration.ConceptsCount = lattice.FormalConcepts.Count();
            StorageService.UpdateConfiguration(configuration);

            // storing to neo4j db
            StoreLatticeInNeo4jDb(lattice);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RegenerateLattice()
        {
            // Callculating lattice
            var lattice = CallculateLattice();

            var configuration = StorageService.GetConfiguration();
            configuration.LatticeCalculationTime = DateTime.Now;
            configuration.LatticeHeight = lattice.Height;
            configuration.ObjectsCount = lattice.FormalContext.ObjectsSet.Count();
            configuration.AttributesCount = lattice.FormalContext.AttributesSet.Count();
            configuration.ConceptsCount = lattice.FormalConcepts.Count();
            StorageService.UpdateConfiguration(configuration);

            // storing to neo4j db
            StoreLatticeInNeo4jDb(lattice);
            return RedirectToAction("ControlPanel");
        }

        public IActionResult Neo4jUpdate(ConfigurationAndStatistics configuration)
        {
            StorageService.UpdateConfiguration(configuration);
            return RedirectToAction("ControlPanel");
        }

        private ConceptLattice CallculateLattice()
        {
            StorageService.MovieLimit = 500;
            var movies = StorageService.GetAllMovies().ToList(); // taking 100
            var categories = StorageService.GetAllCategories().ToList();
            var movieCategories = StorageService.GetAllMovieCategories().ToList();

            var _movies = new Dictionary<int, string>();
            movies.ForEach(m => _movies[m.ID] = m.Name);
            var _categories = new Dictionary<int, string>();
            categories.ForEach(c => _categories[c.ID] = c.Title);

            var objects_d = new Dictionary<string, Object>();
            var attributes_d = new Dictionary<string, Attribute>();

            var objects = movies.Select(m => new Object(m.Name)).ToList();
            objects.ForEach(o => objects_d[o.Name] = o);
            var attributes = categories.Select(c => new Attribute(c.Title)).ToList();
            attributes.ForEach(a => attributes_d[a.Name] = a);

            var objectsAttributes = new Dictionary<Object, HashSet<Attribute>>();
            objects.ForEach(o => objectsAttributes[o] = new HashSet<Attribute>());
            var attributesObjects = new Dictionary<Attribute, HashSet<Object>>();
            attributes.ForEach(a => attributesObjects[a] = new HashSet<Object>());
            foreach (var movieCategory in movieCategories)
            {
                string movie = null;
                _movies.TryGetValue(movieCategory.MovieId, out movie);
                if (movie == null)
                    continue;
                var _object = objects_d[movie];

                var category = _categories[movieCategory.CategoryId];
                var _attribute = attributes_d[category];

                var objectAttributes = objectsAttributes[_object];
                objectAttributes.Add(_attribute);

                var attributeObjects = attributesObjects[_attribute];
                attributeObjects.Add(_object);
            }

            var formalContext = new FormalContext(objects, attributes, objectsAttributes, attributesObjects);
            var nextClousure = new NextClosureAlgorithm(formalContext);
            var formalConcepts = nextClousure.FormalConcepts();
            var lattice = new ConceptLattice(formalConcepts, formalContext);
            return lattice;
        }

        private void StoreLatticeInNeo4jDb(ConceptLattice lattice)
        {
            Neo4JDataProvider.ClearDatabase();
            Neo4JDataProvider.ImportFCALatticeLikeCSV(lattice);
        }

        private IEnumerable<Movie> GetRecomendedMovies(int movieId)
        {
            var movie = StorageService.GetMovie(movieId);
            var movieCategories = StorageService.GetMovieCategories(movieId);

            var recomendedmatches = Neo4JDataProvider.SearchForObjects(movie.Name, movieCategories.Select(c => c.Title));
            var movienames = new List<string>();
            foreach (var match in recomendedmatches.OrderBy(rm => rm.Matches))
            {
                movienames.AddRange(match.Node.Objects);
            }
            movienames = movienames.GroupBy(m => m).Select(g => g.First()).ToList();
            movienames.Remove(movie.Name);
            var recomended = StorageService.GetMoviesByNames(movienames);
            return recomended;
        }

        private IEnumerable<Movie> GetRecomendedMovies(IEnumerable<Movie> allMovies)
        {


            var recomended = new List<string>();
            var user = userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                if (ProcessThread == null || (!ProcessThread.IsAlive))
                {
                    ProcessThread = new Thread(ProcessLikedMovies);
                    ProcessThread.Start();
                }
                if (!string.IsNullOrEmpty(user.SuggestedMovies))
                {
                    recomended = user.SuggestedMovies.Split(",").ToList();
                }
            }

            // when there is no recommendations
            if (recomended.Count == 0)
            {
                return allMovies;
            }
            var recomended_ = recomended.Select(m => int.Parse(m));
                var result = allMovies.Where(m => recomended_.Contains(m.ID)).ToDictionary(r => r.ID, r => r);
            return recomended_.Select(r => result[r]);
        }

        private void ProcessLikedMovies()
        {
            try
            {
                var user = StorageService.dbContext.Users.FirstOrDefault(u => u.Id == UserId);
                if (user == null) return;

                var userLikedMovies = StorageService.LikedMovies(UserId).ToList();
                var likedAttributes = userLikedMovies
                    .SelectMany(m => m.MovieCategories, (m, c) => new { m = m, mc = c })
                    .GroupBy(s => s.mc.Category).Select(m => new AttributeLikes { Likes = m.Count(), Attribute = m.Key.Title })
                    .OrderByDescending(al => al.Likes);
                var recomendedmovies = Neo4JDataProvider.SearchForObjects(likedAttributes, userLikedMovies.Select(m => m.Name));
                var _rm = StorageService.GetMoviesByNames(recomendedmovies).ToDictionary(m => m.Name, m => m.ID);
                user.SuggestedMovies = string.Join(",", recomendedmovies.Select(m => _rm[m]));
                StorageService.dbContext.Users.Update(user);
                StorageService.dbContext.SaveChanges();
            }
            catch(Exception e)
            {

            }
        }

    }
}
