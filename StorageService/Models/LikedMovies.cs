using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace StorageService.Models
{
    public class LikedMovies
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }
        public string UserId { get; set; }
        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }

    }
}
