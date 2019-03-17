using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace StorageService.Models
{
    public class Movie
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string DataLink { get; set; }
        public string Director { get; set; }
        public string Abstract { get; set; }
        public virtual ICollection<MovieCategory> MovieCategories { get; set; }

        public Movie()
        {
            this.MovieCategories = new HashSet<MovieCategory>();
        }
    }
}

