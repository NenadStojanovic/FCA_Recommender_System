using StorageService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCA_Recommender_System.Models
{
    public class HomeIndexViewModel
    {
        public IList<Movie> Movies { get; set; }
        public IList<Category> Categories { get; set; }
    }
}
