using StorageService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCA_Recommender_System.Models
{
    public class ControlPanelViewModel
    {
        public ConfigurationAndStatistics ConfigurationAndStatistics { get; set; }
        public int NumOfMovies { get; set; }
    }
}
