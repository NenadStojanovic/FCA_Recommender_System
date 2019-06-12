using StorageService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCA_Recommender_System.Models
{
    public class MovieViewModel
    {
        public Movie Movie { get; set; }
        public IList<Category> Categories { get; set; }

        public bool Liked { get; set; }
        public int Likes { get; set; }

        public IList<Movie> RecomendedMovies { get; set; }
    }
}
