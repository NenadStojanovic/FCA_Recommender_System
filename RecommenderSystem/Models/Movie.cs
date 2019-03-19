using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderSystem.Models
{
    public class Movie
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string DataLink { get; set; }
        public HashSet<string> Categories { get; set; }

        public Movie(int iD, string name, string dataLink)
        {
            ID = iD;
            Name = name;
            DataLink = dataLink;
            this.Categories = new HashSet<string>();
        }
    }
}
