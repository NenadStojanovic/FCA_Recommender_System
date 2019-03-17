using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace StorageService.Models
{
    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }
        public int Title { get; set; }
        public virtual ICollection<MovieCategory> MovieCategories { get; set; }

        public Category()
        {
            this.MovieCategories = new HashSet<MovieCategory>();
        }
    }
}
