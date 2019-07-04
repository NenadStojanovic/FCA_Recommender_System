using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace StorageService.Models
{
    public class ConfigurationAndStatistics
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }
        public string Neo4jConnectionString { get; set; }
        public string Neo4jUsername { get; set; }
        public string Neo4jPass { get; set; }

        public int ObjectsCount { get; set; }
        public int AttributesCount { get; set; }
        public int ConceptsCount { get; set; }

        public DateTime LatticeCalculationTime { get; set; }
        public int LatticeHeight { get; set; }
        [DisplayName("Number of Movies For Lattice Calculation")]
        public int NumOfMoviesForCalculation { get; set; }
    }
}
